using HAI_Shared;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace OmniLinkBridge.MQTT
{
    public static class MappingExtensions
    {
        public static string ToTopic(this clsArea area, Topic topic)
        {
            return $"{Global.mqtt_prefix}/area{area.Number}/{topic}";
        }

        public static Alarm ToConfig(this clsArea area)
        {
            Alarm ret = new Alarm
            {
                unique_id = $"{Global.mqtt_prefix}area{area.Number}",
                name = Global.mqtt_discovery_name_prefix + area.Name,
                state_topic = area.ToTopic(Topic.basic_state),
                command_topic = area.ToTopic(Topic.command)
            };
            return ret;
        }

        public static string ToState(this clsArea area)
        {
            if (area.AreaAlarms.IsBitSet(0) ||  // Burgulary
                area.AreaAlarms.IsBitSet(3) ||  // Auxiliary
                area.AreaAlarms.IsBitSet(6))    // Duress
                return "triggered";
            else if (area.ExitTimer > 0)
                return "pending";

            return area.AreaMode switch
            {
                enuSecurityMode.Night => "armed_night",
                enuSecurityMode.NightDly => "armed_night_delay",
                enuSecurityMode.Day => "armed_home",
                enuSecurityMode.DayInst => "armed_home_instant",
                enuSecurityMode.Away => "armed_away",
                enuSecurityMode.Vacation => "armed_vacation",
                _ => "disarmed",
            };
        }

        public static string ToBasicState(this clsArea area)
        {
            if (area.AreaAlarms.IsBitSet(0) ||  // Burgulary
                area.AreaAlarms.IsBitSet(3) ||  // Auxiliary
                area.AreaAlarms.IsBitSet(6))    // Duress
                return "triggered";
            else if (area.ExitTimer > 0)
                return "pending";

            switch (area.AreaMode)
            {
                case enuSecurityMode.Night:
                case enuSecurityMode.NightDly:
                    return "armed_night";
                case enuSecurityMode.Day:
                case enuSecurityMode.DayInst:
                    return "armed_home";
                case enuSecurityMode.Away:
                case enuSecurityMode.Vacation:
                    return "armed_away";
                case enuSecurityMode.Off:
                default:
                    return "disarmed";
            }
        }

        public static BinarySensor ToConfigBurglary(this clsArea area)
        {
            BinarySensor ret = new BinarySensor
            {
                unique_id = $"{Global.mqtt_prefix}area{area.Number}burglary",
                name = $"{Global.mqtt_discovery_name_prefix}{area.Name} Burglary",
                device_class = BinarySensor.DeviceClass.safety,
                state_topic = area.ToTopic(Topic.json_state),
                value_template = "{% if value_json.burglary_alarm %} ON {%- else -%} OFF {%- endif %}"
            };
            return ret;
        }

        public static BinarySensor ToConfigFire(this clsArea area)
        {
            BinarySensor ret = new BinarySensor
            {
                unique_id = $"{Global.mqtt_prefix}area{area.Number}fire",
                name = $"{Global.mqtt_discovery_name_prefix}{area.Name} Fire",
                device_class = BinarySensor.DeviceClass.smoke,
                state_topic = area.ToTopic(Topic.json_state),
                value_template = "{% if value_json.fire_alarm %} ON {%- else -%} OFF {%- endif %}"
            };
            return ret;
        }

        public static BinarySensor ToConfigGas(this clsArea area)
        {
            BinarySensor ret = new BinarySensor
            {
                unique_id = $"{Global.mqtt_prefix}area{area.Number}gas",
                name = $"{Global.mqtt_discovery_name_prefix}{area.Name} Gas",
                device_class = BinarySensor.DeviceClass.gas,
                state_topic = area.ToTopic(Topic.json_state),
                value_template = "{% if value_json.gas_alarm %} ON {%- else -%} OFF {%- endif %}"
            };
            return ret;
        }

        public static BinarySensor ToConfigAux(this clsArea area)
        {
            BinarySensor ret = new BinarySensor
            {
                unique_id = $"{Global.mqtt_prefix}area{area.Number}auxiliary",
                name = $"{Global.mqtt_discovery_name_prefix}{area.Name} Auxiliary",
                device_class = BinarySensor.DeviceClass.problem,
                state_topic = area.ToTopic(Topic.json_state),
                value_template = "{% if  value_json.burglary_alarm %} ON {%- else -%} OFF {%- endif %}"
            };
            return ret;
        }

        public static BinarySensor ToConfigFreeze(this clsArea area)
        {
            BinarySensor ret = new BinarySensor
            {
                unique_id = $"{Global.mqtt_prefix}area{area.Number}freeze",
                name = $"{Global.mqtt_discovery_name_prefix}{area.Name} Freeze",
                device_class = BinarySensor.DeviceClass.cold,
                state_topic = area.ToTopic(Topic.json_state),
                value_template = "{% if value_json.freeze_alarm %} ON {%- else -%} OFF {%- endif %}"
            };
            return ret;
        }

        public static BinarySensor ToConfigWater(this clsArea area)
        {
            BinarySensor ret = new BinarySensor
            {
                unique_id = $"{Global.mqtt_prefix}area{area.Number}water",
                name = $"{Global.mqtt_discovery_name_prefix}{area.Name} Water",
                device_class = BinarySensor.DeviceClass.moisture,
                state_topic = area.ToTopic(Topic.json_state),
                value_template = "{% if value_json.water_alarm %} ON {%- else -%} OFF {%- endif %}"
            };
            return ret;
        }

        public static BinarySensor ToConfigDuress(this clsArea area)
        {
            BinarySensor ret = new BinarySensor
            {
                unique_id = $"{Global.mqtt_prefix}area{area.Number}duress",
                name = $"{Global.mqtt_discovery_name_prefix}{area.Name} Duress",
                device_class = BinarySensor.DeviceClass.safety,
                state_topic = area.ToTopic(Topic.json_state),
                value_template = "{% if value_json.duress_alarm %} ON {%- else -%} OFF {%- endif %}"
            };
            return ret;
        }

        public static BinarySensor ToConfigTemp(this clsArea area)
        {
            BinarySensor ret = new BinarySensor
            {
                unique_id = $"{Global.mqtt_prefix}area{area.Number}temp",
                name = $"{Global.mqtt_discovery_name_prefix}{area.Name} Temp",
                device_class = BinarySensor.DeviceClass.heat,
                state_topic = area.ToTopic(Topic.json_state),
                value_template = "{% if value_json.temperature_alarm %} ON {%- else -%} OFF {%- endif %}"
            };
            return ret;
        }

        public static string ToJsonState(this clsArea area)
        {
            AreaState state = new AreaState()
            {
                arming = area.ExitTimer > 0,
                burglary_alarm = area.AreaAlarms.IsBitSet(0),
                fire_alarm = area.AreaAlarms.IsBitSet(1),
                gas_alarm = area.AreaAlarms.IsBitSet(2),
                auxiliary_alarm = area.AreaAlarms.IsBitSet(3),
                freeze_alarm = area.AreaAlarms.IsBitSet(4),
                water_alarm = area.AreaAlarms.IsBitSet(5),
                duress_alarm = area.AreaAlarms.IsBitSet(6),
                temperature_alarm = area.AreaAlarms.IsBitSet(7)
            };

            state.mode = area.AreaMode switch
            {
                enuSecurityMode.Night => "night",
                enuSecurityMode.NightDly => "night_delay",
                enuSecurityMode.Day => "home",
                enuSecurityMode.DayInst => "home_instant",
                enuSecurityMode.Away => "away",
                enuSecurityMode.Vacation => "vacation",
                _ => "off",
            };
            return JsonConvert.SerializeObject(state);
        }

        public static string ToTopic(this clsZone zone, Topic topic)
        {
            return $"{Global.mqtt_prefix}/zone{zone.Number}/{topic}";
        }

        public static Sensor ToConfigTemp(this clsZone zone, enuTempFormat format)
        {
            Sensor ret = new Sensor
            {
                unique_id = $"{Global.mqtt_prefix}zone{zone.Number}temp",
                name = $"{Global.mqtt_discovery_name_prefix}{zone.Name} Temp",
                device_class = Sensor.DeviceClass.temperature,
                state_topic = zone.ToTopic(Topic.current_temperature),
                unit_of_measurement = (format == enuTempFormat.Fahrenheit ? "°F" : "°C")
            };
            return ret;
        }

        public static Sensor ToConfigHumidity(this clsZone zone)
        {
            Sensor ret = new Sensor
            {
                unique_id = $"{Global.mqtt_prefix}zone{zone.Number}humidity",
                name = $"{Global.mqtt_discovery_name_prefix}{zone.Name} Humidity",
                device_class = Sensor.DeviceClass.humidity,
                state_topic = zone.ToTopic(Topic.current_humidity),
                unit_of_measurement = "%"
            };
            return ret;
        }

        public static Sensor ToConfigSensor(this clsZone zone)
        {
            Sensor ret = new Sensor
            {
                unique_id = $"{Global.mqtt_prefix}zone{zone.Number}",
                name = Global.mqtt_discovery_name_prefix + zone.Name
            };

            switch (zone.ZoneType)
            {
                case enuZoneType.EntryExit:
                case enuZoneType.X2EntryDelay:
                case enuZoneType.X4EntryDelay:
                    ret.icon = "mdi:door";
                    break;
                case enuZoneType.Perimeter:
                    ret.icon = "mdi:window-closed";
                    break;
                case enuZoneType.Tamper:
                    ret.icon = "mdi:shield";
                    break;
                case enuZoneType.AwayInt:
                case enuZoneType.NightInt:
                    ret.icon = "mdi:walk";
                    break;
                case enuZoneType.Water:
                    ret.icon = "mdi:water";
                    break;
                case enuZoneType.Fire:
                    ret.icon = "mdi:fire";
                    break;
                case enuZoneType.Gas:
                    ret.icon = "mdi:gas-cylinder";
                    break;
            }

            ret.value_template = @"{{ value|replace(""_"", "" "")|title }}";

            ret.state_topic = zone.ToTopic(Topic.state);
            return ret;
        }

        public static BinarySensor ToConfig(this clsZone zone)
        {
            BinarySensor ret = new BinarySensor
            {
                unique_id = $"{Global.mqtt_prefix}zone{zone.Number}binary",
                name = Global.mqtt_discovery_name_prefix + zone.Name
            };

            Global.mqtt_discovery_override_zone.TryGetValue(zone.Number, out OverrideZone override_zone);

            if (override_zone != null)
            {
                ret.device_class = override_zone.device_class;
            }
            else
            {
                switch (zone.ZoneType)
                {
                    case enuZoneType.EntryExit:
                    case enuZoneType.X2EntryDelay:
                    case enuZoneType.X4EntryDelay:
                        ret.device_class = BinarySensor.DeviceClass.door;
                        break;
                    case enuZoneType.Perimeter:
                        ret.device_class = BinarySensor.DeviceClass.window;
                        break;
                    case enuZoneType.Tamper:
                        ret.device_class = BinarySensor.DeviceClass.problem;
                        break;
                    case enuZoneType.AwayInt:
                    case enuZoneType.NightInt:
                        ret.device_class = BinarySensor.DeviceClass.motion;
                        break;
                    case enuZoneType.Water:
                        ret.device_class = BinarySensor.DeviceClass.moisture;
                        break;
                    case enuZoneType.Fire:
                        ret.device_class = BinarySensor.DeviceClass.smoke;
                        break;
                    case enuZoneType.Gas:
                        ret.device_class = BinarySensor.DeviceClass.gas;
                        break;
                }
            }

            ret.state_topic = zone.ToTopic(Topic.basic_state);
            return ret;
        }

        public static string ToState(this clsZone zone)
        {
            if (zone.Status.IsBitSet(5))
                return "bypassed";
            else if (zone.Status.IsBitSet(2))
                return "tripped";
            else if (zone.Status.IsBitSet(4))
                return "armed";
            else if (zone.Status.IsBitSet(1))
                return "trouble";
            else if (zone.Status.IsBitSet(0))
                return "not_ready";
            else
                return "secure";
        }

        public static string ToBasicState(this clsZone zone)
        {
            return zone.Status.IsBitSet(0) ? "ON" : "OFF";
        }

        public static string ToTopic(this clsUnit unit, Topic topic)
        {
            return $"{Global.mqtt_prefix}/unit{unit.Number}/{topic}";
        }

        public static Light ToConfig(this clsUnit unit)
        {
            Light ret = new Light
            {
                unique_id = $"{Global.mqtt_prefix}unit{unit.Number}light",
                name = Global.mqtt_discovery_name_prefix + unit.Name,
                state_topic = unit.ToTopic(Topic.state),
                command_topic = unit.ToTopic(Topic.command),
                brightness_state_topic = unit.ToTopic(Topic.brightness_state),
                brightness_command_topic = unit.ToTopic(Topic.brightness_command)
            };
            return ret;
        }

        public static Switch ToConfigSwitch(this clsUnit unit)
        {
            Switch ret = new Switch
            {
                unique_id = $"{Global.mqtt_prefix}unit{unit.Number}switch",
                name = Global.mqtt_discovery_name_prefix + unit.Name,
                state_topic = unit.ToTopic(Topic.state),
                command_topic = unit.ToTopic(Topic.command)
            };
            return ret;
        }

        public static string ToState(this clsUnit unit)
        {
            return unit.Status == 0 || unit.Status == 100 ? UnitCommands.OFF.ToString() : UnitCommands.ON.ToString();
        }

        public static int ToBrightnessState(this clsUnit unit)
        {
            if (unit.Status > 100)
                return (ushort)(unit.Status - 100);
            else if (unit.Status == 1)
                return 100;
            else
                return 0;
        }

        public static string ToSceneState(this clsUnit unit)
        {
            if (unit.Status >= 2 && unit.Status <= 13)
                // 2-13 maps to scene A-L respectively
                return ((char)(unit.Status + 63)).ToString();

            return string.Empty;
        }

        public static string ToTopic(this clsThermostat thermostat, Topic topic)
        {
            return $"{Global.mqtt_prefix}/thermostat{thermostat.Number}/{topic}";
        }

        public static Sensor ToConfigTemp(this clsThermostat thermostat, enuTempFormat format)
        {
            Sensor ret = new Sensor
            {
                unique_id = $"{Global.mqtt_prefix}thermostat{thermostat.Number}temp",
                name = $"{Global.mqtt_discovery_name_prefix}{thermostat.Name} Temp",
                device_class = Sensor.DeviceClass.temperature,
                state_topic = thermostat.ToTopic(Topic.current_temperature),
                unit_of_measurement = (format == enuTempFormat.Fahrenheit ? "°F" : "°C")
            };
            return ret;
        }

        public static Sensor ToConfigHumidity(this clsThermostat thermostat)
        {
            Sensor ret = new Sensor
            {
                unique_id = $"{Global.mqtt_prefix}thermostat{thermostat.Number}humidity",
                name = $"{Global.mqtt_discovery_name_prefix}{thermostat.Name} Humidity",
                device_class = Sensor.DeviceClass.humidity,
                state_topic = thermostat.ToTopic(Topic.current_humidity),
                unit_of_measurement = "%"
            };
            return ret;
        }

        public static Climate ToConfig(this clsThermostat thermostat, enuTempFormat format)
        {
            Climate ret = new Climate
            {
                modes = thermostat.Type switch
                {
                    enuThermostatType.AutoHeatCool => new List<string>(new string[] { "auto", "off", "cool", "heat" }),
                    enuThermostatType.HeatCool => new List<string>(new string[] { "off", "cool", "heat" }),
                    enuThermostatType.HeatOnly => new List<string>(new string[] { "off", "heat" }),
                    enuThermostatType.CoolOnly => new List<string>(new string[] { "off", "cool" }),
                    _ => new List<string>(new string[] { "off" }),
                }
            };

            if (format == enuTempFormat.Celsius)
            {
                ret.min_temp = "7";
                ret.max_temp = "35";
            }

            ret.unique_id = $"{Global.mqtt_prefix}thermostat{thermostat.Number}";
            ret.name = Global.mqtt_discovery_name_prefix + thermostat.Name;

            ret.action_topic = thermostat.ToTopic(Topic.current_operation);
            ret.current_temperature_topic = thermostat.ToTopic(Topic.current_temperature);

            ret.temperature_low_state_topic = thermostat.ToTopic(Topic.temperature_heat_state);
            ret.temperature_low_command_topic = thermostat.ToTopic(Topic.temperature_heat_command);

            ret.temperature_high_state_topic = thermostat.ToTopic(Topic.temperature_cool_state);
            ret.temperature_high_command_topic = thermostat.ToTopic(Topic.temperature_cool_command);

            ret.mode_state_topic = thermostat.ToTopic(Topic.mode_state);
            ret.mode_command_topic = thermostat.ToTopic(Topic.mode_command);

            ret.fan_mode_state_topic = thermostat.ToTopic(Topic.fan_mode_state);
            ret.fan_mode_command_topic = thermostat.ToTopic(Topic.fan_mode_command);

            ret.hold_state_topic = thermostat.ToTopic(Topic.hold_state);
            ret.hold_command_topic = thermostat.ToTopic(Topic.hold_command);
            return ret;
        }

        public static string ToOperationState(this clsThermostat thermostat)
        {
            if (thermostat.HorC_Status.IsBitSet(0))
                return "heating";
            else if (thermostat.HorC_Status.IsBitSet(1))
                return "cooling";
            else
                return "idle";
        }

        public static string ToTopic(this clsButton button, Topic topic)
        {
            return $"{Global.mqtt_prefix}/button{button.Number}/{topic}";
        }

        public static Switch ToConfig(this clsButton button)
        {
            Switch ret = new Switch
            {
                unique_id = $"{Global.mqtt_prefix}button{button.Number}",
                name = Global.mqtt_discovery_name_prefix + button.Name,
                state_topic = button.ToTopic(Topic.state),
                command_topic = button.ToTopic(Topic.command)
            };
            return ret;
        }

        public static string ToTopic(this clsMessage message, Topic topic)
        {
            return $"{Global.mqtt_prefix}/message{message.Number}/{topic}";
        }

        public static string ToState(this clsMessage message)
        {
            if (message.Status == enuMessageStatus.Displayed)
                return "displayed";
            else if (message.Status == enuMessageStatus.NotAcked)
                return "displayed_not_acknowledged";
            else
                return "off";
        }
    }
}
