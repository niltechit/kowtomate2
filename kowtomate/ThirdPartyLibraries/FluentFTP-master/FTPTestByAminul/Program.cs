using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using FluentFTP;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


var token = new CancellationToken();
using (var conn = new AsyncFtpClient("192.168.1.9", "developer", "s3PrkU6$")) {
	await conn.Connect(token);

	//conn.Encoding = System.Text.Encoding.GetEncoding(1251);
	//await conn.SetWorkingDirectory("/");
	var filePath = "/KTM Emergency Order Place/Clients/SOS/24-01-16_Eterna_MODEL_1_4_12_FOR TEST_Rakib1baba_New";
	//string encodedPath = HttpUtility.UrlEncode("/KTM Emergency Order Place/Clients/SOS/24-01-16_Eterna_MODEL_1_4_12_FOR TEST_New/7602_92DP05[115457]");
	// Use passive mode if necessary
	//conn.Co = FtpDataConnectionType.PASV;

	// Set the encoding to UTF8 if your server supports it
	//conn.Encoding = System.Text.Encoding.UTF8;

	// example codes
	//FtpClient ftp = ...;

	//string ftppath = "[].folder";
	//string ftppath = "[a].folder";

	//FtpListItem[] list = null;
	//if (path.Contains("[") && path.Contains("]")) {
	await conn.SetWorkingDirectory(filePath);
	//	list = ftp.GetListing(null, FtpListOption.NoPath);
	//}
	//else {
	//	list = ftp.GetListing(ftppath);
	//}
	var files = await conn.GetListing(null, FtpListOption.Recursive | FtpListOption.ForceList | FtpListOption.NoPath);

	// get a recursive listing of the files & folders in a specific folder
	foreach (var item in files) {

		switch (item.Type) {

			case FtpObjectType.Directory:

				Console.WriteLine("Directory!  " + item.FullName);
				Console.WriteLine("Modified date:  " + await conn.GetModifiedTime(item.FullName, token));

				break;

			case FtpObjectType.File:

				Console.WriteLine("File!  " + item.FullName);
				Console.WriteLine("File size:  " + await conn.GetFileSize(item.FullName, -1, token));
				Console.WriteLine("Modified date:  " + await conn.GetModifiedTime(item.FullName, token));
				Console.WriteLine("Chmod:  " + await conn.GetChmod(item.FullName, token));

				break;

			case FtpObjectType.Link:
				break;
		}
	}

}
