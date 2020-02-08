﻿using System;
using System.IO;

namespace TestHelpers.DiffAssertions.DefaultImplementations
{
    internal class TestFileManager : ITestFileManager
    {
        /// <summary>
        /// When a new (expected) file is created a text with short instructions
        /// is shown. This is a helpful way of preventing a nasty problem that seems
        /// to occur when trying to save an empty string into the file.
        /// For some reason an empty string doesn't ensure that the file is created
        /// with the specified UTF-8 encoding (it ends up in Windows 1252 on some machines)!
        /// </summary>
        private const string NewExpectedFileText = 
@"********  DiffAssertions  ********
Check the actual content carefully
and then sync it with the expected
content for future use
**********************************";
        private readonly string _rootFolder;
        private readonly Lazy<DirectoryInfo> _tempDirectoryForStringComparisons = new Lazy<DirectoryInfo>(() =>
            {
                var directory = new DirectoryInfo("[DiffAssertions]");
                if(!directory.Exists)
                    directory.Create();

                return directory;
            });

        public TestFileManager(string rootFolder)
        {
            _rootFolder = rootFolder ?? throw new ArgumentNullException(nameof(rootFolder));
        }

        public ITestFile GetExpectedFile(string fileName)
        {
            var fullName = Path.Combine(_rootFolder, $"{fileName}.expected.txt");
            var expectedFile = new FileInfo(fullName); 

            if (!expectedFile.Exists)
            {
                expectedFile.WriteAllText(NewExpectedFileText);
            }

            return new TestFile(expectedFile);
        }

        public ITestFile CreateTemporaryExpectedFile(string expectedValue, string nameOfFileWithExpectedResult = null)
        {
            if (nameOfFileWithExpectedResult == null)
                nameOfFileWithExpectedResult = Guid.NewGuid().ToString();

            var fullName = Path.Combine(_tempDirectoryForStringComparisons.Value.FullName, $"{nameOfFileWithExpectedResult}.expected.txt");
            var expectedFile = new FileInfo(fullName);
            expectedFile.WriteAllText(expectedValue);

            return new TestFile(expectedFile);
        }

        public ITestFile CreateActualFile(ITestFile expectedFile, string actualValue)
        {
            var fileName = expectedFile.Name.Replace(".expected.txt", ".actual.txt");
            var fullName = Path.Combine(_tempDirectoryForStringComparisons.Value.FullName, fileName);
            var actualFile = new FileInfo(fullName);
            actualFile.WriteAllText(actualValue);

            return new TestFile(actualFile);
        }
    }
}