// See https://aka.ms/new-console-template for more information
using Renci.SshNet;

string sourcefile = @"C:\Users\anush\Documents\TestData.csv";
string targetdir = @"\tst\";
Console.WriteLine("Hello, World!");

string fileName = Path.GetFileName(sourcefile);
string targetFilePath = $"{targetdir}{fileName}";
using (SftpClient client = new SftpClient("secureftp.serco-na.com", "smtint", "M1jwIQzZ369"))
{
    client.Connect();

    //List all files and directories in target remote directory
    //string[] files = client.ListDirectory(targetdir).Select(x => x.FullName).ToArray();

    //Change to working directory in remote
    client.ChangeDirectory(targetdir);
    
    //Upload file to remote
    using (FileStream fs = new FileStream(sourcefile, FileMode.Open))
    {
        client.BufferSize = 4 * 1024;
        client.UploadFile(fs, Path.GetFileName(sourcefile));
    }

    
    // Read content of file in remote
    string content;

    //content = client.ReadAllText(targetFilePath);
    //Console.WriteLine(content);

    using (var remoteFileStream = client.OpenRead(targetFilePath))
    {
        using (var reader = new StreamReader(remoteFileStream))
        {
            content = reader.ReadToEnd();
            Console.WriteLine(content);
        }
    }


    //Delete file from remote directory

    // Have disconnect and reconnect before deleting so that connection can be released from previous task. 
    // Else operations like delete, Exist, ListDirectory throws "No such file" error
    client.Disconnect(); 
    client.Connect();

    if (client.Exists(targetFilePath))
    {
        client.DeleteFile(targetFilePath);
        Console.WriteLine(targetFilePath);
    }
}
