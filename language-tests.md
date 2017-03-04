# Convert CSharp to PowerShell
## Assign a string to a variable
### Source: CSharp
```csharp
namespace CSharpToPowerShell.Test.Languages.CSharp
{
    public class Class
    {
        public void Method()
        {
            var variable = "1";
        }
    }
}

```
### Target: PowerShell
```powershell
function Method
{
	$variable = "1"
}
```

## Assign a constant to a variable
### Source: CSharp
```csharp
namespace CSharpToPowerShell.Test.Languages.CSharp
{
    public class Class
    {
        public void Method()
        {
            var variable = 1;
        }
    }
}

```
### Target: PowerShell
```powershell
function Method
{
	$variable = 1
}
```

## Declare a method
### Source: CSharp
```csharp
namespace CSharpToPowerShell.Test.Languages.CSharp
{
    public class Class
    {
        public void Method()
        {
        }
    }
}

```
### Target: PowerShell
```powershell
function Method
{
}
```

## Declare a method with arguments
### Source: CSharp
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToPowerShell.Test.Languages.CSharp
{
    public class Class
    {
        public void Method(string argument, int integer)
        {
        }
    }
}

```
### Target: PowerShell
```powershell
function Method
{
	param([string]$argument, [int]$integer)
}
```

## Create an object
### Source: CSharp
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToPowerShell.Test.Languages.CSharp
{
    public class Class
    {
        public void Method()
        {
            new System.Object();
        }
    }
}

```
### Target: PowerShell
```powershell
function Method
{
	(New-Object -TypeName System.Object)
}
```

## Create an object with arugments
### Source: CSharp
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToPowerShell.Test.Languages.CSharp
{
    public class Class
    {
        public void Method()
        {
            new System.Object(myVariable);
        }
    }
}

```
### Target: PowerShell
```powershell
function Method
{
	(New-Object -TypeName System.Object -ArgumentList $myVariable)
}
```

## Declare a method outside of a class or namespace
### Source: CSharp
```csharp
void Method()
{

}
```
### Target: PowerShell
```powershell
function Method
{
}
```

