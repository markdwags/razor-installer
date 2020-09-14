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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace RazorInstaller.Helpers
{
    public static class GitHubHelper
    {
        private static readonly GitHubClient _client = new GitHubClient(new ProductHeaderValue("razor-installer"));

        public static Release GetLatestRazorVersion(bool preview)
        {
            IReadOnlyList<Release> releases = _client.Repository.Release.GetAll("markdwags", "Razor").Result;
            return releases.FirstOrDefault(release => release.Prerelease == preview);
        }

        public static Release GetLatestClassicUOVersion(bool preview)
        {
            IReadOnlyList<Release> releases = _client.Repository.Release.GetAll("andreakarasho", "ClassicUO").Result;
            return releases.FirstOrDefault(release => release.Prerelease == preview);
        }
    }
}