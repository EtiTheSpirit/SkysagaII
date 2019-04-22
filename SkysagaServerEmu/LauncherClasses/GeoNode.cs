using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkysagaServerEmu.LauncherClasses {
	/// <summary>
	/// This class is part of the launcher itself. It represents one of the several CDN servers hosted by SkySaga. It was designed to find the closest server to use for downloads.
	/// All data in this class has been directly converted over from the original launcher, with some tidying up and deobfuscation.
	/// </summary>
	class GeoNode : IComparable {

		public List<long> Pings;

		public long MedianPing = 9223372036854775807L;

		public string DataCentre { get; set; }

		public string IP { get; set; }

		public int Port { get; set; }

		public string UUID { get; set; }

		public int CompareTo(object obj) {
			GeoNode other = obj as GeoNode;
			if (other.MedianPing > MedianPing) {
				return -1;
			}
			else if (other.MedianPing == MedianPing) {
				return 0;
			}
			return 1;
		}
	}
}
