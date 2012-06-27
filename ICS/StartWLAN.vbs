'Dror Gluska (2012) - http://uhurumkate.blogspot.co.il/
Option Explicit

'check that Hosted Network Supported = yes
if (CheckHostedNetworkSupported() = false) then
    wsh.echo "Hosted network is not supported"
    wsh.quit
end if
'read from settings the key for ip range
dim iprange, regiprange, test
iprange = ReadSettings("settings.txt","IPRANGE")
wsh.echo "Requested IP Range: ", iprange

regiprange = ReadReg("HKLM\SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\ScopeAddress")
wsh.echo "Existing IP Range: ", regiprange

'if is different, restart the ICS service
if (regiprange <> iprange) then
    wsh.echo "Requested IP Range is different than existing, setting registry"
    test = WriteReg("HKLM\SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\ScopeAddress",iprange,"REG_SZ")
    test = WriteReg("HKLM\SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\ScopeAddressBackup",iprange,"REG_SZ")
    test = WriteReg("HKLM\SYSTEM\CurrentControlSet\services\SharedAccess\Parameters\StandaloneDhcpAddress",iprange,"REG_SZ")

    wsh.echo "Restarting Internet Connection Sharing"
    ExecuteCMD "net stop SharedAccess" 
    ExecuteCMD "net start SharedAccess" 
end if

'read from settings the SSID 
dim ssid
ssid = ReadSettings("settings.txt","SSID")
wsh.echo "Requested SSID: ", ssid

'read from settings the KEY
dim apkey
apkey = ReadSettings("settings.txt","KEY")
wsh.echo "Requested KEY: ", apkey

'starting access point
ExecuteCMD "netsh wlan set hostednetwork mode=allow ssid=" &  ssid & " key=" & apkey
ExecuteCMD "netsh wlan start hostednetwork"

'wait for 2 seconds
wsh.echo "Waiting for hostednetwork to start"
wsh.sleep 2000

'setting static IP for virtual adapter
'getting virtual adapter connection name
dim vaname
vaname = FindActiveConnection("Microsoft Virtual WiFi Miniport Adapter")
if (vaname = "") then
    wsh.echo "unable to find virtual wifi adapter"
    wsh.quit
end if

'set static ip in virtual adapter
ExecuteCMD "netsh interface ip set address name=""" & vaname & """ source=static addr=" & iprange & " mask=255.255.255.0"
ExecuteCMD "netsh interface ip set dns name=""" & vaname & """ source=static addr=none register=PRIMARY"
ExecuteCMD "netsh interface ip set wins name=""" & vaname & """ source=static addr=none"


'getting active connection
dim activecon
activecon =  GetActiveConnection()

if (activecon = "") then
    wsh.echo "unable to get active connection"
    wsh.quit
end if

wsh.echo "Starting Internet Connection Sharing"
wsh.sleep 1000
ExecuteCMD "cscript ics.vbs """ & activecon & """ """ & vaname & """ true /nologo"








'Dim Temp

'HKEY_CURRENT_USER = HKCU
'HKEY_LOCAL_MACHINE = HKLM
'HKEY_CLASSES_ROOT = HKCR
'HKEY_USERS = HKEY_USERS
'HKEY_CURRENT_CONFIG = HKEY_CURRENT_CONFIG

'Temp = WriteReg("HKCU\VBSriptAdmin\Teststring","Success!","REG_SZ")
'Temp = ReadReg("HKCU\VBSriptAdmin\Teststring")
'WScript.Echo Temp
Function WriteReg(RegPath, Value, RegType)
      'Regtype should be "REG_SZ" for string, "REG_DWORD" for a integer,…
      '"REG_BINARY" for a binary or boolean, and "REG_EXPAND_SZ" for an expandable string
      Dim objRegistry, Key
      Set objRegistry = CreateObject("Wscript.shell")

      Key = objRegistry.RegWrite(RegPath, Value, RegType)
      WriteReg = Key
End Function
Function ReadReg(RegPath)
      Dim objRegistry, Key
      Set objRegistry = CreateObject("Wscript.shell")

      Key = objRegistry.RegRead(RegPath)
      ReadReg = Key
End Function

'Check if hostednetwork is supported
function CheckHostedNetworkSupported()
    dim objShell, strCmd, objExecObject, strLine, supported
    supported = false

    Set objShell = CreateObject("Wscript.Shell")

    strCmd = "%comspec% /c netsh wlan show drivers"

    ' Run the command.
    Set objExecObject = objShell.Exec(strCmd)

    ' Read the output
    Do Until objExecObject.StdOut.AtEndOfStream
      strLine = trim(objExecObject.StdOut.ReadLine())
      ' Look for the header line for first interface
      If (InStr(strLine, "Hosted network supported  : Yes") > 0) Then
        supported = true
        exit do
      end if
    Loop

    CheckHostedNetworkSupported = supported
end function

'read a setting from filename
function ReadSettings(filename, settingname)
    dim retval

    dim spitems, strNextLine,objTextFile,objFSO
    Const ForReading = 1
    Set objFSO = CreateObject("Scripting.FileSystemObject")
    Set objTextFile = objFSO.OpenTextFile (filename, ForReading)
    Do Until objTextFile.AtEndOfStream
        strNextLine = objTextFile.Readline
        spitems = split( strNextLine,"=")
        if (spitems(0) = settingname) then
            retval = spitems(1)
            exit do
        end if
    Loop
    objTextFile.close

    ReadSettings = retval
end function

'execute cmd
function ExecuteCMD(cmdtext)
    dim objShell, strCmd, objExecObject,strLine
    Set objShell = CreateObject("Wscript.Shell")

    strCmd = "%comspec% /c " & cmdtext
    'wsh.echo "Executing ", strcmd

    Set objExecObject = objShell.Exec(strCmd)

    Do Until objExecObject.StdOut.AtEndOfStream
        strLine = trim(objExecObject.StdOut.ReadLine())
        wsh.echo strline
    loop

end function

'find active connections via routing table interface list, it will go one by one and check if it has an active connection
function GetActiveConnection()

    dim connname
    Dim objShell, strCmd, objExecObject, strLine, arrValues, strInterface

    dim interfaceList()
    redim interfaceList(0)

    dim startInterfaces
    startInterfaces = false

    wscript.echo "Checking rounting table..."

    Set objShell = CreateObject("Wscript.Shell")
    strCmd = "%comspec% /c route print 8.8.8.8"

    ' Run the command.
    Set objExecObject = objShell.Exec(strCmd)

    ' Read the output
    Do Until objExecObject.StdOut.AtEndOfStream
      strLine = trim(objExecObject.StdOut.ReadLine())
      ' Look for the header line for first interface
      If (InStr(strLine, "Interface List") > 0) Then
        startInterfaces = true
      end if
      if (startInterfaces = true) then
        strInterface = mid(strLine,30)

        'check if end of interfaces
        if (instr(strLine,"==") > 0) then
            Exit Do
        end if

        'add found interface to array
        if (trim(strInterface) <> "") then
            interfaceList(UBound(interfaceList)) = strInterface
            ReDim Preserve interfaceList(UBound(interfaceList) + 1)
        end if

      End If
    Loop

    dim ia

    wscript.echo "Detecting Connection name for interfaces..."

    for each ia in interfaceList
        Wscript.Echo "Interface Name: " & ia

        connname = FindActiveConnection(ia)
        if (connname <> "") then
            exit for
        end if
    next

    GetActiveConnection = connname

end function


function FindActiveConnection(interfaceName)

    dim strCmd,objShell, objExecObject,strLine


    Set objShell = CreateObject("Wscript.Shell")
    strCmd = "%comspec% /c ipconfig /all"


    ' Run the command.
    Set objExecObject = objShell.Exec(strCmd)

    ' Read the output

    Dim adapterName, adapterDescription
    dim foundAdapter, mediaDisconnected
    foundAdapter = false
    mediaDisconnected = false

    Do Until objExecObject.StdOut.AtEndOfStream
        strLine = objExecObject.StdOut.ReadLine()
        ' Look for the header line for each interface
        if (InStr(strLine,"adapter") > 0) then

            if ((adapterDescription = interfaceName) and (mediaDisconnected = false)) then
                foundAdapter = true
                exit do
            end if

            adapterName = trim( mid(strLine,InStr(strLine,"adapter") + 8))
            adapterName = left(adapterName,len(adapterName) -1)
      
            adapterDescription = ""
            mediaDisconnected = false

        end if

        if (Instr(strLine,"Description . . . . . . . . . . . :") > 0) then
            adapterDescription = trim(mid(strLine,39))
        end if

        if (instr(strLine,"Media State . . . . . . . . . . . : Media disconnected") > 0) then
            mediaDisconnected = true
        end if
     
  
    Loop

    if ((adapterDescription = interfaceName) and (mediaDisconnected = false)) then
        foundAdapter = true
    end if


    if (foundAdapter = true) then
        Wscript.Echo "Connection Name: " & adapterName
         FindActiveConnection = adapterName
    end if
end function


