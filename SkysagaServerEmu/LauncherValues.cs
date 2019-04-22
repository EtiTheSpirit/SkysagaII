using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkysagaServerEmu {
	/// <summary>
	/// Represents the values stored in the launcher, some of which are passed into the game when it is launched.
	/// </summary>
	public class LauncherValues {

		// Fun fact: The obfuscator went through ***A LOT*** of trouble to obfuscate get and set on these fields in the launcher code.
		// ...Except it was a system config value and thus had to use the DefaultSettingsValue attribute, which exposed the data.
		// Good work.

		// All datatypes are correct. There are some weird discrepancies (e.g. reset_char_name_link is a string but reset_password_link and account_link are URIs)
		// Any comments with "SPECULATION" before them are, hence their name, simple speculation and are NOT CONFIRMED IN ANY WAY AT ALL.

		/// <summary>
		/// <para>The username of the user launching the game. This argument is passed into SkySaga.exe via a json array and is required for it to launch.</para>
		/// </summary>
		public string Username { get; set; } = "";

		/// <summary>
		/// <para>The password of the user launching the game. This argument is passed into SkySaga.exe via a json array and is required for it to launch.</para>
		/// </summary>
		public string Password { get; set; } = "";

		/// <summary>
		/// <para>Purpose is unknown.</para>
		/// <para>SPECULATION: This is a game server url. "ws" may be short for "World Server". This URL is similar to the game server url accessed by SkySaga.exe (pcdevelop.vm.rw). The differences between the two internally are unknown.</para>
		/// </summary>
		public Uri ws_host { get; set; } = new Uri("http://pcreleaseqa.vm.rw");

		/// <summary>
		/// <para>Purpose is unknown.
		/// Presumably disables all debugging functions and utilities.</para>
		/// <para>SPECULATION: Disables most if not all logging from the launcher.</para>
		/// </summary>
		public bool all_debug_off { get; set; } = false;

		/// <summary>
		/// <para>Purpose is unknown.
		/// Presumably represents a URL related to some source of client data, however nothing more can be gathered.</para>
		/// <para>SPECULATION: This is the site that hosts update packages for the launcher and game.</para>
		/// </summary>
		public string client_url { get; set; } = "http://d1frbonevwpmsv.cloudfront.net/playtest";

		/// <summary>
		/// <para>Purpose is unknown.</para>
		/// <para>SPECULATION: Given the standard usage of "sgauth", this may be used to store userdata e.g. launcher usage, how often you play, etc.</para>
		/// </summary>
		public bool sgauth { get; set; } = true;

		/// <summary>
		/// <para>The webpage that displays on the launcher that displays update info and the login screen.</para>
		/// </summary>
		public Uri home_page { get; set; } = new Uri("http://d1frbonevwpmsv.cloudfront.net/launcher/rcqa/webpages");

		/// <summary>
		/// <para>Purpose is unknown.
		/// "ftue" commonly refers to "First-Time User Experience", which may mean this page was designed for brand new users. It was never used during the alpha test.</para>
		/// </summary>
		public string ftue_page { get; set; } = "about:blank";

		/// <summary>
		/// <para>The name of a text document storing some form of cached data.</para>
		/// <para>The location of this file and what data is stored inside is unknown.</para>
		/// </summary>
		public string clientcache { get; set; } = "ClientCache.rcqa.txt";

		/// <summary>
		/// <para>Purpose is unknown.
		/// No speculation is able to be made on this property at the moment.</para>
		/// </summary>
		public bool main_cr_enable { get; set; } = true;

		/// <summary>
		/// <para>Purpose is unknown.</para>
		/// <para>SPECULATION: This is used to handle developer access, different tiers had access during beta development.</para>
		/// </summary>
		public string tier { get; set; } = "rcqa";

		/// <summary>
		/// <para>The Amazon S3 Key used to access storage on Amazon Web Storage Services. Presumably stores launcher-related data.</para>
		/// <para>The default value was gathered via decomp as part of the stock program resources.</para>
		/// </summary>
		public string s3Key { get; set; } = "AKIAIIRPPFT4QB7BF2TA";

		/// <summary>
		/// <para>The private Amazon S3 Key used to access storage on Amazon Web Storage Services. Presumably stores launcher-related data.</para>
		/// <para>The default value was gathered via decomp as part of the stock program resources.</para>
		/// </summary>
		public string s3SecretKey { get; set; } = "nkNlTCrZrg3zRcigqRDdf+8rnzImDZzI62Iqk70f";

		/// <summary>
		/// <para>Purpose is unknown.</para>
		/// <para>SPECULATION: Points to a plain-text variant of the game's changelog</para>
		/// </summary>
		public string updates_txt_url { get; set; } = "";

		/// <summary>
		/// <para>Purpose is unknown.
		/// Finding out what "st" refers to may reveal more about this property.</para>
		/// </summary>
		public bool st_on_crash { get; set; } = false;

		/// <summary>
		/// <para>Purpose is unknown.
		/// The value is identical to the client_url property, and by extension, may serve the same purpose.</para>
		/// <para>Assuming speculation on client_url is correct, this url points to a site storing game and launcher update packages.</para>
		/// </summary>
		public string launcher_url { get; set; } = "http://d1frbonevwpmsv.cloudfront.net/playtest";

		/// <summary>
		/// <para>Determines whether or not the launcher will store errors related to authentication (e.g. incorrect password) in launcher.log</para>
		/// </summary>
		public bool log_login_errors { get; set; } = false;

		/// <summary>
		/// <para>Purpose is unknown.
		/// Presumably stores the URL used to change your character's name or file a request to do so.</para>
		/// <para>There was no public means of changing your character name provided in the launcher, and this property was never used in production.</para>
		/// </summary>
		public string reset_char_name_link { get; set; } = "";

		/// <summary>
		/// <para>Stores a link to reset your password. Used in the "Forgot Password?" option on the login screen.</para>
		/// </summary>
		public Uri reset_password_link { get; set; } = null;

		/// <summary>
		/// <para>Purpose is unknown.</para>
		/// <para>SPECULATION: Stores a link to access your profile on the main webpage OR accesses some other form of account data stored on the website.</para>
		/// </summary>
		public Uri account_link { get; set; } = null;

	}
}
