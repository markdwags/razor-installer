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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace RazorInstaller.Helpers
{
    public static class InstallerHelper
    {
        private static List<string> _validFiles = new List<string>
        {
            "anim.idx",
            "anim.mul",
            "animdata.mul",
            "cliloc.enu",
            "hues.mul",
            "light.mul",
            "lightidx.mul",
            "multi.idx",
            "multi.mul",
            "radarcol.mul",
            "skills.idx",
            "skills.mul",
            "speech.mul",
            "staidx0.mul",
            "statics0.mul",
            "tiledata.mul"
        };

        public static void ValidatePath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Check if a path can be identified as a valid UO installation path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckUoPath(string path)
        {
            if (!Directory.Exists(path))
                return false;

            List<string> files = new List<string>();

            foreach (string file in Directory.GetFiles(path))
            {
                files.Add(Path.GetFileName(file));
            }

            if (files.Count < 10)
                return false;

            foreach (string validFile in _validFiles)
            {
                string match = files.Find(x => x.StartsWith(validFile, StringComparison.InvariantCultureIgnoreCase));

                if (string.IsNullOrEmpty(match))
                {
                    return false;
                }
            }

            return true;
        }

        public static void UpdateSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.
                OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}