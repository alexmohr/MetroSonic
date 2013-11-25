// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageOutput.cs" company="Alexander Mohr">
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, 
//   or (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
// </copyright>
// <summary>
//   Constants for the translation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace MetroSonic.Content
{
    /// <summary>
    /// Constants for the translation.
    /// </summary>
    /// Todo: All language should be definded here.
    internal static class LanguageOutput
    {
        /// <summary>
        /// All Error Messages.
        /// </summary>
        public struct Errors
        {
            /// <summary>
            /// The download failed.
            /// </summary>
            public const string DownloadFailed = "The download from the subsonic server failed!";

            /// <summary>
            /// The not initialazied message.
            /// </summary>
            public const string LoginFailedLong =
                "Sorry! The login to the server failed. Please check your connection settings";

            /// <summary>
            /// The login failed short.
            /// </summary>
            public const string LoginFailedShort = "Login Failed!";

            /// <summary>
            /// Title for playback errors.
            /// </summary>
            public const string PlaybackErrorTitle = "Playback Error!";
        }

        /// <summary>
        /// All questions.
        /// </summary>
        public struct Questions
        {
            /// <summary>
            /// The settings realy save.
            /// </summary>
            public const string RealySaveMessage
                =
                "Do you realy want to save the settings?\r\n" +
                "If you change the Network settings you have to restart the application";
        }

        /// <summary>
        /// All Warning messages.
        /// </summary>
        public struct Warnings
        {
            /// <summary>
            /// The warnings playlist empty message.
            /// </summary>
            public const string PlaylistEmpty = "You have to add Items to the Playlist first.";

            /// <summary>
            /// The movie not supported.
            /// </summary>
            public const string MovieNotSupported = "Sorry Videos arn't supported.";

            /// <summary>
            /// Title for warning messageboxes.
            /// </summary>
            public const string WarningTitle = "Warning";
        }

        public struct Notice
        {
            public const string ConnectionSuccessfull = "Connection to server successfull";
            public const string GettingArtists = "Getting Artist and Folder List.";

            public const string GettingArtistsCover =
                "Gettings Cover of all Artist this will take a while and the programm will freeze";
        }
    }
}