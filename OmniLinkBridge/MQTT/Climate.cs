using System.Collections.Generic;

namespace OmniLinkBridge.MQTT
{
    public class Climate : Device
    {
        public string action_topic { get; set; }
        public string current_temperature_topic { get; set; }

        public string temperature_low_state_topic { get; set; }
        public string temperature_low_command_topic { get; set; }

        public string temperature_high_state_topic { get; set; }
        public string temperature_high_command_topic { get; set; }

        public string min_temp { get; set; } = "45";
        public string max_temp { get; set; } = "95";

        public string mode_state_topic { get; set; }
        public string mode_command_topic { get; set; }
        public List<string> modes { get; set; } = new List<string>(new string[] { "auto", "off", "cool", "heat" });

        public string fan_mode_state_topic { get; set; }
        public string fan_mode_command_topic { get; set; }
        public List<string> fan_modes { get; set; } = new List<string>(new string[] { "auto", "on", "cycle" });

        public string hold_state_topic { get; set; }
        public string hold_command_topic { get; set; }
    }
}
