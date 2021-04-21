Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Xml
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Security
Imports System.Text.RegularExpressions
Imports System.Web

Module GetMakResult

    Public ReadOnly BPrivateKey As Byte() = New Byte() {&HFE, &H31, &H98, &H75, &HFB, &H48, &H84, &H86, &H9C, &HF3, &HF1, &HCE, &H99, &HA8, &H90, &H64, &HAB, &H57, &H1F, &HCA, &H47, &H4, &H50, &H58, &H30, &H24, &HE2, &H14, &H62, &H87, &H79, &HA0}
    Public Function BuildActivationRequest(PID As String) As String
        Dim envelope As String = Nothing
        Using stream = New MemoryStream()
            Dim utf8 As Encoding = New UTF8Encoding(False)
            Using writer = New XmlTextWriter(stream, utf8)
                writer.WriteStartDocument()
                writer.WriteStartElement("ActivationRequest") '-
                writer.WriteAttributeString("xmlns", "", Nothing, "http://www.microsoft.com/DRM/SL/BatchActivationRequest/1.0")
                writer.WriteStartElement("VersionNumber") '-
                writer.WriteString("2.0")
                writer.WriteEndElement() '-
                writer.WriteStartElement("RequestType") '-
                writer.WriteString("2")
                writer.WriteEndElement() '-
                writer.WriteStartElement("Requests") '-
                writer.WriteStartElement("Request") '--
                writer.WriteStartElement("PID") '---
                writer.WriteString(PID)
                writer.WriteEndElement() '---
                writer.WriteEndElement() '--
                writer.WriteEndElement() '-
                writer.WriteEndElement() '-
            End Using
            envelope = Encoding.UTF8.GetString(stream.ToArray())
        End Using
        Debug.Print(envelope)
        Return envelope
    End Function
    Public Function BuildBatchActivate(Digest As String, RequestXml As String) As String
        Dim envelope As String = Nothing
        Using stream = New MemoryStream()
            Dim utf8 As Encoding = New UTF8Encoding(False)
            Using writer = New XmlTextWriter(stream, utf8)
                writer.WriteStartDocument()
                writer.WriteStartElement("soap:Envelope") '-
                writer.WriteAttributeString("xmlns", "soap", Nothing, "http://schemas.xmlsoap.org/soap/envelope/")
                writer.WriteAttributeString("xmlns", "xsd", Nothing, "http://www.w3.org/2001/XMLSchema")
                writer.WriteAttributeString("xmlns", "xsi", Nothing, "http://www.w3.org/2001/XMLSchema-instance")
                writer.WriteStartElement("soap", "Body", Nothing) '--
                writer.WriteStartElement("BatchActivate", "http://www.microsoft.com/BatchActivationService") '---
                writer.WriteStartElement("request") '----
                writer.WriteStartElement("Digest") '-----
                writer.WriteString(Digest)
                writer.WriteEndElement() '-----
                writer.WriteStartElement("RequestXml") '-----
                writer.WriteString(RequestXml)
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
    Private Function HmacSHA256(ByVal message() As Byte, Optional ByVal secret() As Byte = Nothing) As String
        Dim encoding = New System.Text.UTF8Encoding()
        If secret Is Nothing Then secret = New Byte() {}
        Using hmacsha256_ As New HMACSHA256(secret)
            Dim hashmessage() As Byte = hmacsha256_.ComputeHash(message)
            Return Convert.ToBase64String(hashmessage)
        End Using
    End Function
    Public Function GetCount(ByVal pid As String) As String
        Dim xmlString = BuildActivationRequest(pid)
        Dim byteXml As Byte() = Encoding.Unicode.GetBytes(xmlString)
        Dim base64Xml As String = Convert.ToBase64String(byteXml)

        'Dim hmacsha256 As New HMACSHA256() With {.Key = BPrivateKey}
        Dim digest As String = HmacSHA256(byteXml, BPrivateKey)

        Dim bytes = Encoding.UTF8.GetBytes(BuildBatchActivate(digest, base64Xml))

        Dim myWebRequest = WebRequest.Create("https://activation.sls.microsoft.com/BatchActivation/BatchActivation.asmx")
        myWebRequest.Method = "POST"
        myWebRequest.ContentType = "text/xml; charset=utf-8"
        myWebRequest.Headers.Add("SOAPAction", "http://www.microsoft.com/BatchActivationService/BatchActivate")

        Try
            Using stream As Stream = myWebRequest.GetRequestStream()
                stream.Write(bytes, 0, bytes.Length)
            End Using

            Dim asyncResult As IAsyncResult = myWebRequest.BeginGetResponse(Nothing, Nothing)
            asyncResult.AsyncWaitHandle.WaitOne()

            Dim soapResult As String
            Using webResponse As WebResponse = myWebRequest.EndGetResponse(asyncResult)
                Using rd As New StreamReader(webResponse.GetResponseStream())
                    soapResult = rd.ReadToEnd()
                End Using
            End Using

            Using soapReader As XmlReader = XmlReader.Create(New StringReader(soapResult))
                soapReader.ReadToFollowing("ResponseXml")
                Dim responseXML As String = HttpUtility.HtmlDecode(soapReader.ReadElementContentAsString())
                Using reader As XmlReader = XmlReader.Create(New StringReader(responseXML))
                    reader.ReadToFollowing("ActivationRemaining")
                    Dim szRemain As String = reader.ReadElementContentAsString()
                    If Convert.ToInt32(szRemain) < 0 Then
                        reader.ReadToFollowing("ErrorCode")
                        Dim errors As String = reader.ReadElementContentAsString()
                        If errors = "0x67" Then
                            Return "0 (Blocked)"
                        End If
                    End If
                    Return szRemain
                End Using
            End Using
        Catch ex As Exception

        End Try
        Return -1
    End Function

End Module
