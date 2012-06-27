''Dror Gluska (2012) - http://uhurumkate.blogspot.co.il/

'detect virtual adapter connection name
dim vaname
vaname = FindActiveConnection("Microsoft Virtual WiFi Miniport Adapter")
if (vaname = "") then
    wsh.echo "unable to find virtual wifi adapter"
    wsh.quit
end if

'set to dynamic ip
ExecuteCMD "netsh interface ip set address name=""" & vaname & """ source=dhcp"
ExecuteCMD "netsh interface ip set dns name=""" & vaname & """ source=dhcp register=PRIMARY"
ExecuteCMD "netsh interface ip set wins name=""" & vaname & """ source=dhcp"

'stop wlan
ExecuteCMD "netsh wlan set hostednetwork mode=disallow"
ExecuteCMD "netsh wlan stop hostednetwork"

'stopping ICS

dim activecon
activecon =  GetActiveConnection()

if (activecon = "") then
    wsh.echo "unable to get active connection"
    wsh.quit
end if

wsh.sleep 1000
ExecuteCMD "cscript ics.vbs """ & activecon & """ """ & vaname & """ false /nologo"


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