'=========================================================================
'Author: Jason Kohl
'Program Name: Manga List
'Description: Stores data and pages of manga, rips images off the internet
'========================================================================
Option Explicit On
Option Strict On

Imports System.Collections.Generic
Imports System.Linq
Imports System.IO

Public Class MainForm

    Structure ListElementRating
        Public mangaLinkedListElement As Integer
        Public mangaLinkedListRating As Integer
    End Structure

    Private Sub Processing(task As String)
        Static mangaList As New List(Of mangaInfo)

        Select Case task
            Case "A" ''Add New Manga Panel
                mainPanel.Visible = False
                newMangaPanel.Visible = True

            Case "S"  ''Search Manga Panel
                searchListPanel.Visible = True
                mainPanel.Visible = False


            Case "D"  ''Download Manga Panel
                downloadMangaPanel.Visible = True
                mainPanel.Visible = False

            Case "L"  ''Load Data
                Dim iFStream As IO.StreamReader

                If (IO.File.Exists("MangaData.txt")) Then
                    iFStream = IO.File.OpenText("MangaData.txt")
                    Do Until iFStream.Peek = -1
                        Dim tempManga As New mangaInfo
                        tempManga.Title = iFStream.ReadLine()
                        Integer.TryParse(iFStream.ReadLine(), tempManga.Chapters)
                        tempManga.Genres = iFStream.ReadLine()
                        Double.TryParse(iFStream.ReadLine(), tempManga.Rating)
                        tempManga.Link = iFStream.ReadLine()
                        tempManga.LocalPicLink = iFStream.ReadLine()
                        tempManga.ReadStat = iFStream.ReadLine()
                        tempManga.CompStat = iFStream.ReadLine()
                        tempManga.TitleWords = iFStream.ReadLine()
                        mangaList.Add(tempManga)
                    Loop
                    iFStream.Close()
                Else
                    MessageBox.Show("No saved data found")
                End If


            Case "AA"  ''Save Data, Manga Info Panel
                Dim tempManga As New mangaInfo
                Dim oFstream As IO.StreamWriter

                ''Collect  Textbox Input
                tempManga.Title = titleTextBox.Text
                Integer.TryParse(chaptersTextBox.Text, tempManga.Chapters)
                tempManga.Genres = generaTextBox.Text
                Double.TryParse(ratingTextBox.Text, tempManga.Rating)
                tempManga.Link = webLinkTextBox.Text
                tempManga.LocalPicLink = picLinkTextBox.Text

                ''Collect Radio Button Input and Clear
                If (wTRRadioButton.Checked = True) Then
                    tempManga.ReadStat = "Want To Read"
                    wTRRadioButton.Checked = False
                ElseIf (readingRadioButton.Checked = True) Then
                    tempManga.ReadStat = "Reading"
                    readingRadioButton.Checked = False
                ElseIf (finishedReadingRadioButton.Checked = True) Then
                    tempManga.ReadStat = "Finsihed"
                    finishedReadingRadioButton.Checked = False
                End If

                If (inProgRadioButton.Checked = True) Then
                    tempManga.CompStat = "In Progress"
                    inProgRadioButton.Checked = False
                ElseIf (finishedRadioButton.Checked = True) Then
                    tempManga.CompStat = "Finished"
                    finishedRadioButton.Checked = False
                ElseIf (hiatusRadioButton.Checked = True) Then
                    tempManga.CompStat = "Hiatus"
                    hiatusRadioButton.Checked = False
                End If

                'Filter Manga title
                Dim titleParts = New List(Of String)
                TitleToWordList(tempManga.Title, titleParts)
                FilterTitleList(titleParts)
                Dim i As Integer = 0
                Dim tempTitle As String = ""
                While (i < titleParts.Count)
                    tempTitle = tempTitle + titleParts(i) + " "
                    i += 1
                End While
                tempManga.TitleWords = tempTitle

                ''Clear Textboxes and Radio Buttons
                ClearMangaTextBoxes()

                ''Add to List
                mangaList.Add(tempManga)

                ''Save to File
                If (IO.File.Exists("MangaData.txt")) Then
                    oFstream = IO.File.AppendText("MangaData.txt")
                Else
                    oFstream = IO.File.CreateText("MangaData.txt")
                End If
                oFstream.WriteLine(tempManga.Title)
                oFstream.WriteLine(tempManga.Chapters)
                oFstream.WriteLine(tempManga.Genres)
                oFstream.WriteLine(tempManga.Rating)
                oFstream.WriteLine(tempManga.Link)
                oFstream.WriteLine(tempManga.LocalPicLink)
                oFstream.WriteLine(tempManga.ReadStat)
                oFstream.WriteLine(tempManga.CompStat)
                oFstream.WriteLine(tempManga.TitleWords)
                oFstream.Close()

                ''Display Info Panel
                searchListPanel.Visible = True
                newMangaPanel.Visible = False

                ''Display Manga Info
                PrintMangaInfo(mangaList, mangaList.Count - 1)


            Case "AC"  ''Add New Manga Panel Cancel
                mainPanel.Visible = True
                newMangaPanel.Visible = False

                ''Clear Textboxes and Radio Buttons
                ClearMangaTextBoxes()

            Case "SS" '' should break down word and search for specifics and related sraeches in final version
                searchListBox.Items.Clear()
                Dim index As Integer = 0
                Dim mFound As Boolean = False
                SearchManga(mangaList, searchTextBox.Text)

            Case "SC" ''search clear
                searchListBox.Items.Clear()
                searchTextBox.Clear()
                mainPanel.Visible = True
                searchListPanel.Visible = False

            Case "SA" ''display all manga
                searchListBox.Items.Clear()
                Dim i As Integer = 0

                While i < mangaList.Count
                    PrintMangaInfo(mangaList, i)
                    i += 1
                End While

            Case "SE" ''edit manga info
                If (searchListBox.SelectedIndex <> -1) Then

                    Dim tempString As String = searchListBox.GetItemText(searchListBox.SelectedItem)
                    If (tempString.Substring(0, 7) = "Title: ") Then

                        ''Switch to manga new/edit pannel
                        newMangaPanel.Visible = True
                        searchListPanel.Visible = False

                        ''Edit pannel to match editing action
                        newMangaLabel.Text = "Edit Manga"
                        addButton.Visible = False
                        saveEditsButton.Visible = True
                        cancelNewMangaButton.Visible = False
                        cancelEditButton.Visible = True

                        ''Copy info into pannel textboxes
                        Dim editIndex As Integer = searchListBox.SelectedIndex()
                        titleTextBox.Text = tempString.Remove(0, 7)
                        editIndex += 1
                        searchListBox.SelectedIndex() = editIndex
                        chaptersTextBox.Text = searchListBox.GetItemText(searchListBox.SelectedItem).Remove(0, 10)
                        editIndex += 1
                        searchListBox.SelectedIndex() = editIndex
                        generaTextBox.Text = searchListBox.GetItemText(searchListBox.SelectedItem).Remove(0, 8)
                        editIndex += 1
                        searchListBox.SelectedIndex() = editIndex
                        ratingTextBox.Text = searchListBox.GetItemText(searchListBox.SelectedItem).Remove(0, 8)
                        editIndex += 1
                        searchListBox.SelectedIndex() = editIndex
                        webLinkTextBox.Text = searchListBox.GetItemText(searchListBox.SelectedItem).Remove(0, 10)
                        editIndex += 1
                        searchListBox.SelectedIndex() = editIndex
                        picLinkTextBox.Text = searchListBox.GetItemText(searchListBox.SelectedItem).Remove(0, 16)

                        ''convert and copy radiobox data
                        editIndex += 1
                        searchListBox.SelectedIndex() = editIndex
                        tempString = searchListBox.GetItemText(searchListBox.SelectedItem).Remove(0, 11)
                        If (tempString = "Want To Read") Then
                            wTRRadioButton.Checked = True
                        ElseIf (tempString = "Reading") Then
                            readingRadioButton.Checked = True
                        Else
                            finishedReadingRadioButton.Checked = True
                        End If

                        editIndex += 1
                        searchListBox.SelectedIndex() = editIndex
                        tempString = searchListBox.GetItemText(searchListBox.SelectedItem).Remove(0, 12)
                        If (tempString = "In Progress") Then
                            inProgRadioButton.Checked = True
                        ElseIf (tempString = "Finished") Then
                            finishedRadioButton.Checked = True
                        Else
                            hiatusRadioButton.Checked = True
                        End If

                    End If

                End If


            Case "SEE" ''Save edits to manga entry

                ''Return index to title selection
                searchListBox.SelectedIndex() = searchListBox.SelectedIndex() - 7

                ''Find entry in mangalist
                Dim i As Integer = 0
                Dim tempTitle As String = searchListBox.GetItemText(searchListBox.SelectedItem).Remove(0, 7)
                While (tempTitle <> mangaList(i).Title AndAlso i < mangaList.Count())
                    i += 1
                End While

                ''Make Edits
                ''Collect  Textbox Input
                mangaList(i).Title = titleTextBox.Text
                Integer.TryParse(chaptersTextBox.Text, mangaList(i).Chapters)
                mangaList(i).Genres = generaTextBox.Text
                Double.TryParse(ratingTextBox.Text, mangaList(i).Rating)
                mangaList(i).Link = webLinkTextBox.Text
                mangaList(i).LocalPicLink = picLinkTextBox.Text

                ''Collect Radio Button Input and Clear
                If (wTRRadioButton.Checked = True) Then
                    mangaList(i).ReadStat = "Want To Read"
                    wTRRadioButton.Checked = False
                ElseIf (readingRadioButton.Checked = True) Then
                    mangaList(i).ReadStat = "Reading"
                    readingRadioButton.Checked = False
                ElseIf (finishedReadingRadioButton.Checked = True) Then
                    mangaList(i).ReadStat = "Finsihed"
                    finishedReadingRadioButton.Checked = False
                End If

                If (inProgRadioButton.Checked = True) Then
                    mangaList(i).CompStat = "In Progress"
                    inProgRadioButton.Checked = False
                ElseIf (finishedRadioButton.Checked = True) Then
                    mangaList(i).CompStat = "Finished"
                    finishedRadioButton.Checked = False
                ElseIf (hiatusRadioButton.Checked = True) Then
                    mangaList(i).CompStat = "Hiatus"
                    hiatusRadioButton.Checked = False
                End If

                'Filter Manga title ===============================can make more efficient by first checking if title has been changed ========================
                Dim titleParts = New List(Of String)
                TitleToWordList(mangaList(i).Title, titleParts)
                FilterTitleList(titleParts)
                Dim j As Integer = 0
                tempTitle = ""
                While (j < titleParts.Count)
                    tempTitle = tempTitle + titleParts(j) + " "
                    j += 1
                End While
                mangaList(i).TitleWords = tempTitle


                ''Save to File
                i = 0
                Dim oFstream As IO.StreamWriter
                If (IO.File.Exists("MangaData.txt")) Then
                    IO.File.Delete("MangaData.txt")
                End If
                oFstream = IO.File.CreateText("MangaData.txt")
                While (i < mangaList.Count())

                    oFstream.WriteLine(mangaList(i).Title)
                    oFstream.WriteLine(mangaList(i).Chapters)
                    oFstream.WriteLine(mangaList(i).Genres)
                    oFstream.WriteLine(mangaList(i).Rating)
                    oFstream.WriteLine(mangaList(i).Link)
                    oFstream.WriteLine(mangaList(i).LocalPicLink)
                    oFstream.WriteLine(mangaList(i).ReadStat)
                    oFstream.WriteLine(mangaList(i).CompStat)
                    oFstream.WriteLine(mangaList(i).TitleWords)
                    i += 1
                End While
                oFstream.Close()


                ''Restore New Manga pannel and Return
                FinishedWithEdits()


            Case "SEC" ''Cancel edits to manga entry

                ''Restore New Manga pannel and Return
                FinishedWithEdits()

                ''Return index to title selection
                searchListBox.SelectedIndex() = searchListBox.SelectedIndex() - 7


            Case "DD"  ''====================================clean this up, we are collecting the url twice, maybe just see if you can see if there is text there instead? ===================
                Dim pictureURL = urlTextBox.Text()
                If (pictureURL <> "") Then
                    Process.Start(pictureURL)
                    WebBrowser1.Navigate(pictureURL)
                End If

            Case "DC"
                downloadMangaPanel.Visible = False
                mainPanel.Visible = True

            Case "Z" ''Add New Manga Panel
                mainPanel.Visible = False
                aboutPanel.Visible = True

            Case "ZC" ''Add New Manga Panel
                mainPanel.Visible = True
                aboutPanel.Visible = False

        End Select
    End Sub


    ''========================================================================Button Processing Calls=======================================================================================
    Private Sub addButton_Click(sender As Object, e As EventArgs) Handles addNewButton.Click
        Processing("A")
    End Sub

    Private Sub searchButton_Click(sender As Object, e As EventArgs) Handles searchPanelButton.Click
        Processing("S")
    End Sub

    Private Sub downloadButton_Click(sender As Object, e As EventArgs) Handles downloadButton.Click
        Processing("D")
    End Sub

    Private Sub exitButton_Click(sender As Object, e As EventArgs) Handles exitButton.Click
        Me.Close()
    End Sub

    Private Sub addButton_Click_1(sender As Object, e As EventArgs) Handles addButton.Click
        Processing("AA")
    End Sub

    Private Sub cancelNewMangaButton_Click(sender As Object, e As EventArgs) Handles cancelNewMangaButton.Click
        Processing("AC")
    End Sub

    Private Sub chaptersTextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles chaptersTextBox.KeyPress
        If ((e.KeyChar < "0" OrElse e.KeyChar > "9") AndAlso (e.KeyChar <> ControlChars.Back)) Then
            e.Handled = True
        End If
    End Sub

    Private Sub ratingTextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles ratingTextBox.KeyPress
        If ((e.KeyChar < "0" OrElse e.KeyChar > "9") AndAlso (e.KeyChar <> ControlChars.Back)) Then
            e.Handled = True
        End If
    End Sub

    Private Sub searchButton_Click_1(sender As Object, e As EventArgs) Handles searchButton.Click
        Processing("SS")
    End Sub

    Private Sub cancelSearchButton_Click(sender As Object, e As EventArgs) Handles cancelSearchButton.Click
        Processing("SC")
    End Sub

    Private Sub showAllButton_Click(sender As Object, e As EventArgs) Handles showAllButton.Click
        Processing("SA")
    End Sub

    Private Sub downloadMangaButton_Click(sender As Object, e As EventArgs) Handles downloadMangaButton.Click
        Processing("DD")
    End Sub

    Private Sub cancelDownloadButton_Click(sender As Object, e As EventArgs) Handles cancelDownloadButton.Click
        Processing("DC")
    End Sub

    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted  ''got this code off of youtube
        Dim PageElements As HtmlElementCollection = WebBrowser1.Document.GetElementsByTagName("img")
        Dim pictureCounter As Integer = 0
        Dim userName As String = Environment.UserName
        Dim directoryPath As String = "C:\Users\" & userName & "\Downloads\MangaListPages"

        ' Check if directory does not exist
        If Not Directory.Exists(directoryPath) Then
            ' Create a new directory
            Directory.CreateDirectory(directoryPath)
        End If

        directoryPath = directoryPath & "\"

        downloadLabel.Visible = True
        downloadLabel.Text = "Downloading..."
        For Each CurElement As HtmlElement In PageElements
            Dim pictureURL = urlTextBox.Text()
            Dim pictureLocation = directoryPath & pictureCounter.ToString() & ".png"
            My.Computer.Network.DownloadFile(CurElement.GetAttribute("src"), pictureLocation)
            pictureCounter += 1
        Next
        downloadLabel.Text = "Download Finished!"
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ''Set Pannel Locations
        newMangaPanel.Location = mainPanel.Location
        searchListPanel.Location = mainPanel.Location
        searchResultsPanel.Location = mainPanel.Location
        downloadMangaPanel.Location = mainPanel.Location
        aboutPanel.Location = mainPanel.Location

        ''Hide Pannels
        newMangaPanel.Visible = False
        searchListPanel.Visible = False
        searchResultsPanel.Visible = False
        downloadMangaPanel.Visible = False
        aboutPanel.Visible = False

        'mainMenuPictureBox.Image() = My.Resources.Header2
        'mainMenuPictureBox.Refresh()

        Processing("L")

    End Sub

    '====================================================================Functions==================================================================================================
    Private Sub PrintMangaInfo(ByRef mangaList As List(Of mangaInfo), i As Integer)
        searchListBox.Items.Add("================================================")
        searchListBox.Items.Add("Title: " + mangaList(i).Title)
        searchListBox.Items.Add("Chapters: " + mangaList(i).Chapters.ToString)
        searchListBox.Items.Add("Genres: " + mangaList(i).Genres)
        searchListBox.Items.Add("Rating: " + mangaList(i).Rating.ToString)
        searchListBox.Items.Add("Web Link: " + mangaList(i).Link)
        searchListBox.Items.Add("Local Pic Link: " + mangaList(i).LocalPicLink)
        searchListBox.Items.Add("Read Stat: " + mangaList(i).ReadStat)
        searchListBox.Items.Add("Manga Stat: " + mangaList(i).CompStat)
        searchListBox.Items.Add("================================================")
    End Sub

    Private Sub SearchManga(ByRef mangaList As List(Of mangaInfo), title As String)
        Dim titlePartsSearch = New List(Of String) '' our title words to search for
        Dim titlePartsEntry = New List(Of String) '' our title words to search for
        Dim mangaRelevanceList As New List(Of ListElementRating) ''higher rank is better
        Dim i As Integer = 0 'Counter for mangaList loop
        Dim j As Integer = 0 'Counter for titlePartsSearch loop
        Dim k As Integer = 0 'Counter for titlePartsEntry loop
        Dim l As Integer = 0 'Counter for mangaReferenceList loop
        Dim matchedKeyWords As Integer = 0
        Dim elementNum As Integer = 0 'may need a structure to hold both pieces of data
        Dim tempListStruct As ListElementRating
        Dim highestRating As Integer = 0

        'Split the searched title up into a list of words
        TitleToWordList(title, titlePartsSearch)

        'Delete any filter words from list
        FilterTitleList(titlePartsSearch)

        While (i < mangaList.Count)
            TitleToWordList(mangaList(i).TitleWords, titlePartsEntry)
            While (j < titlePartsSearch.Count)
                While (k < titlePartsEntry.Count)
                    If (titlePartsSearch(j) = titlePartsEntry(k)) Then
                        matchedKeyWords += 1
                    End If
                    k += 1
                End While
                k = 0
                j += 1
            End While

            If (matchedKeyWords <> 0) Then
                tempListStruct.mangaLinkedListRating = matchedKeyWords
                tempListStruct.mangaLinkedListElement = i
                If (highestRating <= matchedKeyWords) Then 'the equals is so that we can quickly add in ratings that match the highest rating to the beginning of list
                    highestRating = matchedKeyWords
                    mangaRelevanceList.Insert(0, tempListStruct)
                ElseIf (matchedKeyWords = 1) Then
                    mangaRelevanceList.Insert((mangaRelevanceList.Count), tempListStruct)
                Else
                    While (l < mangaRelevanceList.Count AndAlso mangaRelevanceList(l).mangaLinkedListRating > tempListStruct.mangaLinkedListRating)
                        l += 1
                    End While
                    If (l = mangaRelevanceList.Count) Then
                        mangaRelevanceList.Add(tempListStruct)
                    Else
                        mangaRelevanceList.Insert(l, tempListStruct)
                    End If
                    l = 0

                End If
            End If
            matchedKeyWords = 0
            titlePartsEntry.Clear()
            j = 0
            i += 1
        End While

        i = 0
        While (i < mangaRelevanceList.Count)
            PrintMangaInfo(mangaList, mangaRelevanceList(i).mangaLinkedListElement)
            i += 1
        End While

    End Sub


    Private Sub TitleToWordList(title As String, ByRef titleParts As List(Of String))

        Dim wordEnd As Integer = 0

        title = title.ToLower
        title = title.TrimStart
        title = title.TrimEnd
        While (wordEnd < title.Length)
            If (title(wordEnd) = " ") Then 'we will need to check for punctuation as then commamas and shit will be included in the search
                titleParts.Add(title.Substring(0, wordEnd))
                title = title.Remove(0, wordEnd + 1)
                wordEnd = 0
            Else
                wordEnd += 1
            End If
        End While
        titleParts.Add(title.Substring(0, wordEnd))
        title = title.Remove(0, wordEnd)

    End Sub

    Private Sub FilterTitleList(ByRef titleParts As List(Of String))
        Dim filterWords = New String() {"the", "a", "an"}  ''===================================================investigate why filler words such as "is", "of" etc are not in filler list====================================
        Dim fWCounter As Integer = 0
        Dim tPCounter As Integer = 0

        While (fWCounter < filterWords.Count)
            While (tPCounter < titleParts.Count)
                If (filterWords(fWCounter) = titleParts(tPCounter)) Then
                    titleParts.RemoveAt(tPCounter)
                End If
                tPCounter += 1
            End While
            tPCounter = 0
            fWCounter += 1
        End While

    End Sub

    Private Sub ClearMangaTextBoxes()
        ''Clear Textboxes and Radio Buttons
        titleTextBox.Clear()
        chaptersTextBox.Clear()
        generaTextBox.Clear()
        ratingTextBox.Clear()
        webLinkTextBox.Clear()
        picLinkTextBox.Clear()
        wTRRadioButton.Checked = True
        wTRRadioButton.Checked = False
        inProgRadioButton.Checked = True
        inProgRadioButton.Checked = False
    End Sub


    Private Sub FinishedWithEdits()
        ''Restore New Manga pannel
        newMangaLabel.Text = "New Manga"
        addButton.Visible = True
        saveEditsButton.Visible = False
        cancelNewMangaButton.Visible = True
        cancelEditButton.Visible = False

        ''Clear Textboxes and Radio Buttons
        ClearMangaTextBoxes()

        ''Switch to manga search pannel
        newMangaPanel.Visible = False
        searchListPanel.Visible = True

    End Sub


    Private Function randNumString() As String
        Dim randGen As New Random
        Return randGen.Next(0, 99999).ToString()
    End Function

    Private Sub editMangaButton_Click(sender As Object, e As EventArgs) Handles editMangaButton.Click
        Processing("SE")
    End Sub

    Private Sub saveEditsButton_Click(sender As Object, e As EventArgs) Handles saveEditsButton.Click
        Processing("SEE")
    End Sub

    Private Sub cancelEditButton_Click(sender As Object, e As EventArgs) Handles cancelEditButton.Click
        Processing("SEC")
    End Sub

    Private Sub aboutButton_Click(sender As Object, e As EventArgs) Handles aboutButton.Click
        Processing("Z")
    End Sub

    Private Sub cancelAboutButton_Click(sender As Object, e As EventArgs) Handles cancelAboutButton.Click
        Processing("ZC")
    End Sub
End Class



'Things to do...
' create file system for downloads
' improve search effeciency
' add reading option for "reached the end of manga and am waiting for new materal to come out" or something like that
' add ability to upload custom banners
' make it so that random banners show up for each page
' clean up edit button for entries code
'Can possible solve the problem of downloading pictures that are not the pages by checking the dimensions of the pictures, need to research if most manga pages have a similar size, or a very close range
'can use accibility properties to search for "next chapter" button perhaps? or search for the number for the next chapter
' there sould be a negiotive ranking for searches if there are words that do not appear  at all in the search, can also check # of key words in search vs current array checkk, so stuff like committee on education would have 2 key words, while committee on health education has 3, so if your search key words are only 2 long, then obously the 3 long result would not be correct.
'should use the list of arrays to check against the search key words as this would allow more negitive points to be accumliated for terms that are not in the search term, i.e searching the search term against the list would result in committee on education->committee on education to have 2 mathes, but committee on education->committee on health education would also have 2 matches, but if you search the opposite way, then committee on education->committee on education would have two matches and committee on health education->committee on education would have only 1 point, as you would lose a point for health not being in the search key words