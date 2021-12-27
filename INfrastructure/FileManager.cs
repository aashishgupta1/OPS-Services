using OPSService.Infrastructure.Loggging;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace OPSService.Infrastructure
{
    public static class FileManager
    {
        private const string FileValidationPattern = "FileValidationPattern";
        private const string FileValidationWaitTime = "FileValidationWaitTime";
        private static readonly char[] FilePatternSeparator = { ';' };
        private static ILogger logger = Logger.Instance;
        public static bool IsFileAccessible(this string filePath)
        {
            bool result = false;
            FileStream stream = null;
            try
            {
                FileInfo file = new FileInfo(filePath);
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                if (stream != null)
                {
                    stream.Close();
                }
                result = IsFileCopyFinished(filePath);
            }
            catch (Exception e)
            {
                Logger.Instance.LogError(e, e.Message);
                result = false;
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return result;
        }

        private static bool IsFileCopyFinished(string filePath)
        {
            bool status = true;
            try
            {
                var fileCopyCheckRegexesString = System.Configuration.ConfigurationManager.AppSettings.Get(FileValidationPattern);
                if (!string.IsNullOrWhiteSpace(fileCopyCheckRegexesString))
                {
                    var fileCopyCheckRegexes = fileCopyCheckRegexesString.Split(FilePatternSeparator);
                    string fileName = Path.GetFileName(filePath);
                    bool fileNameMatch = false;
                    foreach (var regex in fileCopyCheckRegexes)
                    {
                        if (Regex.Match(fileName, regex, RegexOptions.IgnoreCase).Success)
                        {
                            fileNameMatch = true;
                            break;
                        }
                    }
                    if (fileNameMatch)
                    {
                        string fileCopyWaitingTimeString = System.Configuration.ConfigurationManager.AppSettings.Get(FileValidationWaitTime);
                        if (int.TryParse(fileCopyWaitingTimeString, out int fileCopyWaitingTime) && fileCopyWaitingTime > 0)
                        {
                            status = false;
                            long oldFileSize = GetFileSize(filePath);
                            Logger.Instance.LogDebug("Old File Size: " + oldFileSize);
                            for (int waitIndex = 0; waitIndex < fileCopyWaitingTime; waitIndex++)
                            {
                                Thread.Sleep(1000);
                                long newFileSize = GetFileSize(filePath);
                                Logger.Instance.LogDebug("New File Size: " + newFileSize);
                                if (oldFileSize == newFileSize)
                                {
                                    status = true;
                                    break;
                                }
                                else
                                {
                                    oldFileSize = newFileSize;
                                }
                            }
                        }
                        else
                        {
                            Logger.Instance.LogDebug("Invalid file copy check waiting time value in app.config. Skipping file copy progress check...");
                        }
                    }
                    else
                    {
                        Logger.Instance.LogDebug("File name does not match with any regex in app.config. Skipping file copy progress check...");
                    }
                }
                else
                {
                    Logger.Instance.LogDebug("No file copy check regex present in app.config. Skipping file copy progress check...");
                }
            }
            catch (Exception e)
            {
                Logger.Instance.LogError(e, e.Message);
                throw;
            }
            return status;
        }

        public static bool IsFileEmpty(this string filePath)
        {
            bool result = false;
            if (GetFileSize(filePath) == 0)
            {
                result = true;
            }
            return result;
        }

        private static long GetFileSize(string filePath)
        {
            return new FileInfo(filePath).Length;
        }

        public static void MoveAndReplaceFile(string sourceFilePath, string destinationFilePath)
        {
            try
            {
                if (File.Exists(sourceFilePath))
                {
                    if (File.Exists(destinationFilePath))
                    {
                        logger.LogInfo($"deleting file {destinationFilePath}");
                        File.Delete(destinationFilePath);
                    }
                    logger.LogInfo($"moving file {sourceFilePath} to {destinationFilePath}");
                    File.Move(sourceFilePath, destinationFilePath);
                }
                else
                {
                    logger.LogInfo($"Source file {sourceFilePath} does't Exist.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while moving file {sourceFilePath} to {destinationFilePath}");
            }
        }

        public static void DeleteFile(string filePath)
        {
            logger.LogInfo($"Deleting file {filePath}");
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"error while deleting file {filePath}");
            }
        }
    }
}
