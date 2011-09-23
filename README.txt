MogileFsApi is a C# API for MogileFs (see http://danga.com/mogilefs/ and http://code.google.com/p/mogilefs/).

License
=======
Copyright 2010 Twingly AB. MogileFsApi is provided under the three-clause
BSD License. See the included LICENSE.txt file for specifics.

Examples
========

// Create a simple client against the server mogile1
var uri = new Uri("mogile://mogile1:6001");
var client = new MogileFsClient(new Uri[] { uri });

// Store a file
var d = "testdomain";
var key = "testkey";

using (var filestream = new FileStream(@"C:\tmp\some_file.jpg", FileMode.Open)) {
	client.StoreFile(d, key, "original", filestream, 100000);
}

// Retrieve a file
byte[] arr = client.GetFileBytes(d, key, 10000);
