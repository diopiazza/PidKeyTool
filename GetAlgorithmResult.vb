Imports System.IO
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Xml

Public Class GetAlgorithmResult
    Public Structure AcquisitionInfo
        Public ptr As Long
        Public cbSizeName As Integer
        Public cbSizeValue As Integer
    End Structure
    Public Shared Function BuildEnvelope(TokenType As String, UseKeyDic As Dictionary(Of String, String), ClaimsDic As Dictionary(Of String, String)) As String
        Dim envelope As String = Nothing
        Using stream = New MemoryStream()
            Dim utf8 As Encoding = New UTF8Encoding(False)
            Using writer = New XmlTextWriter(stream, utf8)
                writer.WriteStartDocument()
                writer.WriteStartElement("soap:Envelope") '-
                writer.WriteAttributeString("xmlns", "soapenc", Nothing, "http://schemas.xmlsoap.org/soap/encoding/")
                writer.WriteAttributeString("xmlns", "soap", Nothing, "http://schemas.xmlsoap.org/soap/envelope/")
                writer.WriteAttributeString("xmlns", "xsd", Nothing, "http://www.w3.org/2001/XMLSchema")
                writer.WriteAttributeString("xmlns", "xsi", Nothing, "http://www.w3.org/2001/XMLSchema-instance")
                writer.WriteStartElement("soap", "Body", Nothing) '--
                writer.WriteStartElement("RequestSecurityToken", "http://schemas.xmlsoap.org/ws/2004/04/security/trust") '---
                writer.WriteStartElement("TokenType") '----
                writer.WriteString(TokenType)
                writer.WriteEndElement() '----
                writer.WriteStartElement("RequestType") '----
                writer.WriteString("http://schemas.xmlsoap.org/ws/2004/04/security/trust/Issue")
                writer.WriteEndElement() '----
                writer.WriteStartElement("UseKey") '----
                writer.WriteStartElement("Values") '-----
                If UseKeyDic.Count > 0 Then
                    writer.WriteAttributeString("xmlns", "q1", Nothing, "http://schemas.xmlsoap.org/ws/2004/04/security/trust")
                    writer.WriteAttributeString("soapenc", "arrayType", Nothing, "q1:TokenEntry[" + UseKeyDic.Count.ToString + "]")
                    For Each pair As KeyValuePair(Of String, String) In UseKeyDic
                        writer.WriteStartElement("TokenEntry") '------
                        writer.WriteStartElement("Name") '-------
                        writer.WriteString(pair.Key)
                        writer.WriteEndElement() '-------
                        writer.WriteStartElement("Value") '-------
                        writer.WriteString(pair.Value)
                        writer.WriteEndElement() '-------
                        writer.WriteEndElement() '------
                    Next
                    writer.WriteEndElement() '-----
                    writer.WriteEndElement() '----
                Else
                    writer.WriteAttributeString("xmlns", "nil", Nothing, "1")
                    writer.WriteEndElement() '-----
                    writer.WriteEndElement() '----
                End If
                writer.WriteStartElement("Claims") '----
                writer.WriteStartElement("Values") '-----
                writer.WriteAttributeString("xmlns", "q1", Nothing, "http://schemas.xmlsoap.org/ws/2004/04/security/trust")
                writer.WriteAttributeString("soapenc", "arrayType", Nothing, "q1:TokenEntry[" + ClaimsDic.Count.ToString + "]")
                For Each pair As KeyValuePair(Of String, String) In ClaimsDic
                    writer.WriteStartElement("TokenEntry") '------
                    writer.WriteStartElement("Name") '-------
                    writer.WriteString(pair.Key)
                    writer.WriteEndElement() '-------
                    writer.WriteStartElement("Value") '-------
                    writer.WriteString(pair.Value)
                    writer.WriteEndElement() '-------
                    writer.WriteEndElement() '------
                Next
                writer.WriteEndElement() '-----
                writer.WriteEndElement() '----
                writer.WriteEndElement() '---
                writer.WriteEndElement() '--
                writer.WriteEndElement() '-
            End Using
            envelope = Encoding.UTF8.GetString(stream.ToArray())
        End Using
        Debug.Print(envelope)
        Return envelope
    End Function

    Public Shared Function GetPostData2009(ByVal bytesIn() As Byte, ByRef url As String) As String
        Dim UseKeyDic As New Dictionary(Of String, String)
        Dim ClaimsDic As New Dictionary(Of String, String)
        Do While bytesIn.Length > 0
            Dim size As Integer = Marshal.SizeOf(GetType(AcquisitionInfo))
            Dim buffer As IntPtr = Marshal.AllocHGlobal(size)
            Marshal.Copy(bytesIn, 0, buffer, size)
            Dim pResult As AcquisitionInfo = CType(Marshal.PtrToStructure(buffer, GetType(AcquisitionInfo)), AcquisitionInfo)
            Dim szKey = Encoding.Unicode.GetString(bytesIn, 16, pResult.cbSizeName - 2).Trim
            Dim offsetKey = ((pResult.cbSizeName + 7) And Not 7) - pResult.cbSizeName
            Debug.Print(szKey.Length.ToString + ":" + szKey)
            If szKey = "0:SessionKey" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim '30H
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", "").Trim, szValue)
            ElseIf szKey = "0:BindingType" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:Binding" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:ProductKey" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:ProductKeyType" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:ProductKeyActConfigId" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:SppSvcVersion" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "1:PublishLicense" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                UseKeyDic.Add(szKey.Replace("1:", ""), szValue)
            ElseIf szKey = "0:otherInfoPublic.licenseCategory" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPrivate.licenseCategory" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPublic.sysprepAction" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPrivate.sysprepAction" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "SppLAPServerURL" Then
                url = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(url + vbNewLine)
            ElseIf szKey = "SppLAPRequestTokenType" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
            ElseIf szKey = "0:ClientInformation" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:ReferralInformation" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:ClientSystemTime" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:ClientSystemTimeUtc" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPublic.secureStoreId" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPrivate.secureStoreId" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            End If
            Dim offsetValue = ((pResult.cbSizeValue + 7) And Not 7) - pResult.cbSizeValue
            bytesIn = bytesIn.Skip(&H10 + pResult.cbSizeName + offsetKey + pResult.cbSizeValue + offsetValue).ToArray
        Loop
        Return BuildEnvelope("ProductActivation", UseKeyDic, ClaimsDic)
    End Function

    Public Shared Function GetAlgorithmRCA(ByVal bytesIn() As Byte) As String 'rca:RightsAccountCertificate
        Dim UseKeyDic As New Dictionary(Of String, String)
        Dim ClaimsDic As New Dictionary(Of String, String)
        Do While bytesIn.Length > 0
            Dim size As Integer = Marshal.SizeOf(GetType(AcquisitionInfo))
            Dim buffer As IntPtr = Marshal.AllocHGlobal(size)
            Marshal.Copy(bytesIn, 0, buffer, size)
            Dim pResult As AcquisitionInfo = CType(Marshal.PtrToStructure(buffer, GetType(AcquisitionInfo)), AcquisitionInfo)
            Dim szKey = Encoding.Unicode.GetString(bytesIn, 16, pResult.cbSizeName - 2).Trim
            Debug.Print(szKey.Length.ToString + ":" + szKey)
            Dim offsetKey = ((pResult.cbSizeName + 7) And Not 7) - pResult.cbSizeName
            If szKey = "0:BindingType" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:Binding" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "1:SPCPublicCertificate" Then 'SecurityProcessorCertificate
                Dim SecurityProcessorCertificate = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(SecurityProcessorCertificate.Length.ToString + ":" + SecurityProcessorCertificate)
                UseKeyDic.Add(szKey.Replace("1:", ""), SecurityProcessorCertificate)
            ElseIf szKey = "0:otherInfoPublic.licenseCategory" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPrivate.licenseCategory" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPublic.sysprepAction" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPrivate.sysprepAction" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPublic.racActivationGroup" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPrivate.racActivationGroup" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "SppLAPServerURL" Then
                Dim url = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(url + vbNewLine) 'http://go.microsoft.com/fwlink/?LinkID=88339
            ElseIf szKey = "SppLAPRequestTokenType" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
            End If
            Dim offsetValue = ((pResult.cbSizeValue + 7) And Not 7) - pResult.cbSizeValue
            bytesIn = bytesIn.Skip(&H10 + pResult.cbSizeName + offsetKey + pResult.cbSizeValue + offsetValue).ToArray
        Loop
        Return BuildEnvelope("RAC", UseKeyDic, ClaimsDic)
    End Function

    Public Shared Function GetAlgorithmPKC(ByVal bytesIn() As Byte) As String 'pkc:ProductKeyCertificate
        Dim UseKeyDic As New Dictionary(Of String, String)
        Dim ClaimsDic As New Dictionary(Of String, String)
        Do While bytesIn.Length > 0
            Dim size As Integer = Marshal.SizeOf(GetType(AcquisitionInfo))
            Dim buffer As IntPtr = Marshal.AllocHGlobal(size)
            Marshal.Copy(bytesIn, 0, buffer, size)
            Dim pResult As AcquisitionInfo = CType(Marshal.PtrToStructure(buffer, GetType(AcquisitionInfo)), AcquisitionInfo)
            Dim szKey = Encoding.Unicode.GetString(bytesIn, 16, pResult.cbSizeName - 2).Trim
            Debug.Print(szKey.Length.ToString + ":" + szKey)
            Dim offsetKey = ((pResult.cbSizeName + 7) And Not 7) - pResult.cbSizeName
            If szKey = "0:ProductKey" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:ProductKeyType" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:ProductKeyActConfigId" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "SppLAPServerURL" Then
                Dim url = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(url + vbNewLine)
            ElseIf szKey = "SppLAPRequestTokenType" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
            End If
            Dim offsetValue = ((pResult.cbSizeValue + 7) And Not 7) - pResult.cbSizeValue
            bytesIn = bytesIn.Skip(&H10 + pResult.cbSizeName + offsetKey + pResult.cbSizeValue + offsetValue).ToArray
        Loop
        Return BuildEnvelope("PKC", UseKeyDic, ClaimsDic)
    End Function

    Public Shared Function GetAlgorithmEUL(ByVal bytesIn() As Byte) As String 'eul:UseLicense
        Dim UseKeyDic As New Dictionary(Of String, String)
        Dim ClaimsDic As New Dictionary(Of String, String)
        Dim offset_start = 0
        Do While bytesIn.Length > 0
            Dim size As Integer = Marshal.SizeOf(GetType(AcquisitionInfo))
            Dim buffer As IntPtr = Marshal.AllocHGlobal(size)
            Marshal.Copy(bytesIn, offset_start, buffer, size)
            Dim pResult As AcquisitionInfo = CType(Marshal.PtrToStructure(buffer, GetType(AcquisitionInfo)), AcquisitionInfo)
            Dim szKey = Encoding.Unicode.GetString(bytesIn, 16, pResult.cbSizeName - 2).Trim
            Debug.Print(szKey.Length.ToString + ":" + szKey)
            Dim offsetKey = ((pResult.cbSizeName + 7) And Not 7) - pResult.cbSizeName
            If szKey = "1:SecurityProcessorCertificate" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                UseKeyDic.Add(szKey.Replace("1:", ""), szValue)
            ElseIf szKey = "1:RightsAccountCertificate" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                UseKeyDic.Add(szKey.Replace("1:", ""), szValue)
            ElseIf szKey = "1:ProductKeyCertificate" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                UseKeyDic.Add(szKey.Replace("1:", ""), szValue)
            ElseIf szKey = "1:PublishLicense" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue)
                UseKeyDic.Add(szKey.Replace("1:", ""), szValue)
            ElseIf szKey = "0:otherInfoPublic.licenseCategory" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPrivate.licenseCategory" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPublic.sysprepAction" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPrivate.sysprepAction" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "SppLAPServerURL" Then
                Dim url = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(url + vbNewLine)
            ElseIf szKey = "SppLAPRequestTokenType" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
            ElseIf szKey = "0:ClientInformation" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:ReferralInformation" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:ClientSystemTime" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:ClientSystemTimeUtc" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPublic.secureStoreId" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
                ClaimsDic.Add(szKey.Replace("0:", ""), szValue)
            ElseIf szKey = "0:otherInfoPrivate.secureStoreId" Then
                Dim szValue = Encoding.Unicode.GetString(bytesIn, 16 + pResult.cbSizeName + offsetKey, pResult.cbSizeValue - 2).Trim
                Debug.Print(szValue.Length.ToString + ":" + szValue + vbNewLine)
            End If
            Dim offsetValue = ((pResult.cbSizeValue + 7) And Not 7) - pResult.cbSizeValue
            bytesIn = bytesIn.Skip(&H10 + pResult.cbSizeName + offsetKey + pResult.cbSizeValue + offsetValue).ToArray
        Loop
        Return BuildEnvelope("UseLicense", UseKeyDic, ClaimsDic)
    End Function

End Class
