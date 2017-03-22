$graphics.FillRectangle($brushBg,$x,$y,$width,$height) 

Add-Type -AssemblyName System.Drawing
$filename = "e:\temp\foo.png" 
$bmp = new-object System.Drawing.Bitmap 300,200 
$font = new-object System.Drawing.Font Consolas,16 
$brushBg = [System.Drawing.Brushes]::Yellow 
$brushFg = [System.Drawing.Brushes]::Black 
$graphics = [System.Drawing.Graphics]::FromImage($bmp) 
$x=0; $y=0; $width=300; $height=50
$graphics.FillRectangle($brushBg,$x,$y,$width,$height) 
$graphics.DrawString("Hello`t100",$font,$brushFg,$x+5,$y+($height/4)) 
$brushBg = [System.Drawing.Brushes]::blue 
$brushFg = [System.Drawing.Brushes]::white 
$graphics = [System.Drawing.Graphics]::FromImage($bmp) 
$x=0; $y=51; $width=300; $height=50
$graphics.FillRectangle($brushBg,$x,$y,$width,$height) 
$graphics.DrawString("World`t50",$font,$brushFg,$x+5,$y+($height/4)) 
$graphics.Dispose() 
$bmp.Save($filename)