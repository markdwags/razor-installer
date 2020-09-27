#region license

// Razor Installer
// Copyright (C) 2020 Razor Development Community on GitHub <https://github.com/markdwags/razor-installer>
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using Newtonsoft.Json;
using System.Windows;

namespace RazorInstaller.Models
{
    public class ClassicUoModel
    {
        [JsonProperty("username")] public string Username { get; set; } = string.Empty;

        [JsonProperty("password")] public string Password { get; set; } = string.Empty;

        [JsonProperty("ip")] public string IP { get; set; } = "127.0.0.1";

        [JsonProperty("port")] public int Port { get; set; } = 2593;

        [JsonProperty("ultimaonlinedirectory")]
        public string UltimaOnlineDirectory { get; set; } = "";

        [JsonProperty("clientversion")] public string ClientVersion { get; set; } = string.Empty;

        [JsonProperty("lastcharactername")] public string LastCharacterName { get; set; } = string.Empty;

        [JsonProperty("cliloc")] public string ClilocFile { get; set; } = "Cliloc.enu";

        [JsonProperty("lastservernum")] public ushort LastServerNum { get; set; } = 1;

        [JsonProperty("fps")] public int FPS { get; set; } = 60;
        [JsonProperty("window_position")] public Point? WindowPosition { get; set; }
        [JsonProperty("window_size")] public Point? WindowSize { get; set; }

        [JsonProperty("is_win_maximized")] public bool IsWindowMaximized { get; set; } = true;

        [JsonProperty("saveaccount")] public bool SaveAccount { get; set; }

        [JsonProperty("autologin")] public bool AutoLogin { get; set; }

        [JsonProperty("reconnect")] public bool Reconnect { get; set; }

        [JsonProperty("reconnect_time")] public int ReconnectTime { get; set; }

        [JsonProperty("login_music")] public bool LoginMusic { get; set; } = true;

        [JsonProperty("login_music_volume")] public int LoginMusicVolume { get; set; } = 15;

        [JsonProperty("shard_type")] public int ShardType { get; set; }

        [JsonProperty("fixed_time_step")] public bool FixedTimeStep { get; set; } = true;

        [JsonProperty("run_mouse_in_separate_thread")]
        public bool RunMouseInASeparateThread { get; set; } = true;

        [JsonProperty("force_driver")] public byte ForceDriver { get; set; }

        [JsonProperty("use_verdata")] public bool UseVerdata { get; set; }

        [JsonProperty("encryption")] public byte Encryption { get; set; }

        [JsonProperty("plugins")] public string[] Plugins { get; set; } = { @"./Assistant/Razor.dll" };
    }
}