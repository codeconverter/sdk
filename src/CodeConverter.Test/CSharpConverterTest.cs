using CodeConverter.Common;
using CodeConverter.PowerShell;
using NUnit.Framework;

namespace CodeConverter.Test
{
    [TestFixture]
    public class CSharpConverterTest
    {
        private string Convert(string code)
        {
            Node.DefaultCodeWriter = new PowerShellCodeWriter();
            return new CodeConverterFactory().Convert(code, Language.CSharp, Language.PowerShell);
        }

        [Test]
        public void ShouldIgnoreNamespaces()
        {
            var powershell = Convert("namespace My.Namespace { }");

            Assert.That(powershell, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ShouldReturnANewFunction()
        {
            var powershell = Convert("class Test { void Method() {  } } ");

            Assert.That(powershell, Is.EqualTo("function Method {}"));
        }

        [Test]
        public void ShouldReturnANewFunctionWithParameters()
        {
            var powershell = Convert("class Test { void Method(string name, bool yes) {  } } ");

            Assert.That(powershell, Is.EqualTo("function Method {param([string]$name,[bool]$yes)}"));
        }

        [Test]
        public void ShouldReturnNewObject()
        {
            var powershell = Convert("class Test { void Method() { new System.Object(); } } ");

            Assert.That(powershell, Is.EqualTo("function Method {(New-Object -TypeName System.Object)}"));
        }

        [Test]
        public void ShouldReturnNewObjectWithArguments()
        {
            var powershell = Convert("class Test { void Method(string variable) { new MyType(variable); } } ");

            Assert.That(powershell, Is.EqualTo("function Method {param([string]$variable)(New-Object -TypeName MyType -ArgumentList $variable)}"));
        }

        [Test]
        public void ShouldReturnNewObjectWithMultipleArguments()
        {
            var powershell = Convert("class Test { void Method(string variable) { new MyType(variable, variable); } } ");

            Assert.That(powershell, Is.EqualTo("function Method {param([string]$variable)(New-Object -TypeName MyType -ArgumentList $variable,$variable)}"));
        }


        [Test]
        public void ShouldReturnIntegerVariableAssignment()
        {
            var powershell = Convert("class Test { void Method() { var i = 1; } } ");

            Assert.That(powershell, Is.EqualTo("function Method {$i = 1}"));
        }

        [Test]
        public void ShouldReturnStringVariableAssignment()
        {
            var powershell = Convert("class Test { void Method() { var i = \"hey\"; } } ");

            Assert.That(powershell, Is.EqualTo("function Method {$i = \"hey\"}"));
        }

        [Test]
        public void ShouldReturnMethodExecution()
        {
            var powershell = Convert("class Test { void Method(string myString) { myString.ToString(); } } ");

            Assert.That(powershell, Is.EqualTo("function Method {param([string]$myString)$myString.ToString()}"));
        }

        [Test]
        public void ShouldReturnMethodExecutionWithArguments()
        {
            var powershell = Convert("class Test { void Method(string myString) { myString.ToString(1,2); } } ");

            Assert.That(powershell, Is.EqualTo("function Method {param([string]$myString)$myString.ToString(1,2)}"));
        }

        [Test]
        public void ShouldReturnMethodExecutionWithArgumentsAndVariables()
        {
            var powershell = Convert("class Test { void Method(string myString) { myString.ToString(1,myString); } } ");

            Assert.That(powershell, Is.EqualTo("function Method {param([string]$myString)$myString.ToString(1,$myString)}"));
        }

        [Test]
        public void ShouldAccessProperty()
        {
            var powershell = Convert("class Test { void Method(string myString) { var time = myString.DateTime; } } ");

            Assert.That(powershell, Is.EqualTo("function Method {param([string]$myString)$time = $myString.DateTime}"));
        }

        [Test]
        public void ShouldSetProperty()
        {
            var powershell = Convert("class Test { void Method(string myString) { myString.DateTime = 1; } } ");

            Assert.That(powershell, Is.EqualTo("function Method {param([string]$myString)$myString.DateTime=1}"));
        }

        [Test]
        public void ShouldReturnStaticMethodCall()
        {
            var powershell = Convert("class Test { void Method() { DateTime.Now; } } ");

            Assert.That(powershell, Is.EqualTo("function Method {[DateTime]::Now\r\n}"));
        }

        [Test]
        public void ShouldFixAwaitExpressions()
        {
            var powershell = Convert("class Test { async Task Method(Awaiter a) { await a.DoSomething(); } } ");

            Assert.That(powershell, Is.EqualTo("function Method {param([Awaiter]$a)$a.DoSomething().Result\r\n}"));
        }

        [Test]
        public void ShouldSupportCast()
        {
            var powershell = Convert("class Test { void Method(object a) { var item = (string)a; } } ");

            Assert.That(powershell, Is.EqualTo("function Method {param([object]$a)$item = [string]$a}"));
        }

        [Test]
        public void ShouldSupportIf()
        {
            var powershell = Convert("class Test { void Method() { if (true) { } } } ");

            Assert.That(powershell, Is.EqualTo("function Method {if($true){}}"));
        }

        [Test]
        public void ShouldSupportElse()
        {
            var powershell = Convert("class Test { void Method() { if (true) { } else { } } } ");

            Assert.That(powershell, Is.EqualTo("function Method {if($true){}else{}}"));
        }

        [Test]
        public void ShouldSupportElseIf()
        {
            var powershell = Convert("class Test { void Method() { if (true) { } else if (1 == 2) { } } } ");

            Assert.That(powershell, Is.EqualTo("function Method {if($true){}elseif(1 -eq 2){}}"));
        }

        [Test]
        public void ShouldSupportEq()
        {
            var powershell = Convert("class Test { void Method() { if (1 == 1) { } } } ");

            Assert.That(powershell, Is.EqualTo("function Method {if(1 -eq 1){}}"));
        }

        [Test]
        public void ShouldSupportNe()
        {
            var powershell = Convert("class Test { void Method() { if (1 != 1) { } } } ");

            Assert.That(powershell, Is.EqualTo("function Method {if(1 -ne 1){}}"));
        }

        [Test]
        public void ShouldSupportGt()
        {
            var powershell = Convert("class Test { void Method() { if (1 > 1) { } } } ");

            Assert.That(powershell, Is.EqualTo("function Method {if(1 -gt 1){}}"));
        }

        [Test]
        public void ShouldSupportGe()
        {
            var powershell = Convert("class Test { void Method() { if (1 >= 1) { } } } ");

            Assert.That(powershell, Is.EqualTo("function Method {if(1 -ge 1){}}"));
        }

        [Test]
        public void ShouldSupportLt()
        {
            var powershell = Convert("class Test { void Method() { if (1 < 1) { } } } ");

            Assert.That(powershell, Is.EqualTo("function Method {if(1 -lt 1){}}"));
        }

        [Test]
        public void ShouldSupportLe()
        {
            var powershell = Convert("class Test { void Method() { if (1 <= 1) { } } } ");

            Assert.That(powershell, Is.EqualTo("function Method {if(1 -le 1){}}"));
        }

        [Test]
        public void ShouldSupportAnd()
        {
            var powershell = Convert("class Test { void Method() { if (1 == 1 && 1 == 1) { } } } ");

            Assert.That(powershell, Is.EqualTo("function Method {if(1 -eq 1 -and 1 -eq 1){}}"));
        }

        [Test]
        public void ShouldSupportOr()
        {
            var powershell = Convert("class Test { void Method() { if (1 == 1 || 1 == 1) { } } } ");

            Assert.That(powershell, Is.EqualTo("function Method {if(1 -eq 1 -or 1 -eq 1){}}"));
        }

        [Test]
        public void ShouldSupportNot()
        {
            var powershell = Convert("class Test { void Method() { if (!(1 == 1)) { } } } ");

            Assert.That(powershell, Is.EqualTo("function Method {if(-not (1 -eq 1)){}}"));
        }

        [Test]
        public void StackoverFlowTest()
        {
            var csharp = @" CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
        CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer sourceContainer = cloudBlobClient.GetContainerReference(containerName);
        CloudBlobContainer targetContainer = cloudBlobClient.GetContainerReference(targetContainerName);
        CloudBlockBlob sourceBlob;
        CloudBlockBlob targetBlob;
        foreach (var blobItem in sourceContainer.ListBlobs())
        {
            sourceBlob = sourceContainer.GetBlockBlobReference(blobItem.Uri.ToString());
            targetBlob = targetContainer.GetBlockBlobReference(blobItem.Uri.ToString());
            targetBlob.StartCopy(sourceBlob);
        }";

            //csharp = $"class Test {{ void Method() {{ {csharp} }} }}";

            var powershell = Convert(csharp);
        }

        [Test]
        public void ShouldConvertSnippet()
        {
            var powershell = Convert("void Method() {}");
            Assert.That(powershell, Is.EqualTo("function Method {}"));
        }

        [Test]
        public void ShouldConvertMoreComplexSnippet()
        {
            var powershell = Convert("void Method() {}");
            Assert.That(powershell, Is.EqualTo("function Method {}"));
        }

        [Test]
        public void ShouldConvertTryCatch()
        {
            var powershell = Convert("void Method() {try { new Object(); } catch {}}");
            Assert.That(powershell, Is.EqualTo("function Method {try{(New-Object -TypeName Object)}catch{}}"));
        }

        [Test]
        public void ShouldConvertTryCatchWithType()
        {
            var powershell = Convert("void Method() {try { new Object(); } catch (Exception){}}");
            Assert.That(powershell, Is.EqualTo("function Method {try{(New-Object -TypeName Object)}catch[Exception]{}}"));
        }

        [Test]
        public void ShouldConvertForeach()
        {
            var powershell = Convert("void Method() {foreach(var item in items) {}}");
            Assert.That(powershell, Is.EqualTo("function Method {foreach($item in $items){}}"));
        }

        [Test]
        public void ShouldConvertComments()
        {
            var powershell = Convert("void Method() {//This thing\r\nvar myItem = new Object();}");
            Assert.That(powershell, Is.EqualTo("function Method{$myItem=(New-Object -TypeName Object)\r\n}"));
        }

        [Test]
        public void ShouldConvertReturnStatement()
        {
            var powershell = Convert("void Method() {return;}");
            Assert.That(powershell, Is.EqualTo("function Method {return}"));
        }

        [Test]
        public void ShouldConvertIndexer()
        {
            var powershell = Convert("void Method() {item[1]=1;}");
            Assert.That(powershell, Is.EqualTo("function Method {$item[1]=1}"));
        }

        [Test]
        public void ShouldConvertPlus()
        {
            var powershell = Convert("void Method() {var i = 1 + 1;}");
            Assert.That(powershell, Is.EqualTo("function Method {$i = 1 + 1}"));
        }

        [Test]
        public void ShouldConvertMinus()
        {
            var powershell = Convert("void Method() {var i = 1 - 1;}");
            Assert.That(powershell, Is.EqualTo("function Method {$i = 1 - 1}"));
        }

        [Test]
        public void ShouldConvertBinaryOr()
        {
            var powershell = Convert("void Method() {var i = 1 | 1;}");
            Assert.That(powershell, Is.EqualTo("function Method {$i = 1 -bor 1}"));
        }

        [Test]
        public void ShouldConvertForLoop()
        {
            var powershell = Convert("void Method() { for(var i = 1; i < 100; i++){}}");
            Assert.That(powershell, Is.EqualTo("function Method {for($i = 1;$i -lt 100;$i++){}}"));
        }

        [Test]
        public void ShouldConvertBreak()
        {
            var powershell = Convert("void Method() { for(var i = 1; i < 100; i++){break;}}");
            Assert.That(powershell, Is.EqualTo("function Method {for($i = 1;$i -lt 100;$i++){break}}"));
        }

        [Test]
        public void ShouldSupportUsing()
        {
            var powershell = Convert("void Method() { using{var i = 1;}}");
            Assert.That(powershell, Is.EqualTo("function Method{$i=1\r\n\r\n}"));
        }

        [Test]
        public void ShouldSupportWhile()
        {
            var powershell = Convert("void Method() { while(true){var i = 1;}}");
            Assert.That(powershell, Is.EqualTo("function Method {while($true){$i = 1}}"));
        }
    }
}
