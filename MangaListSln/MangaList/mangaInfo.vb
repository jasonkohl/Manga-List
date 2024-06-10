Public Class mangaInfo
    Private mangaTitle As String
    Private mangaLink As String
    Private mangaReadStat As String
    Private mangaCompStat As String
    Private mangaLocalPicLink As String
    Private mangaRating As Double
    Private mangaGenres As String
    Private mangaChapters As Integer
    Private mangaTitleWords As String

    Public Property Title As String
        Get
            Return mangaTitle
        End Get
        Set(value As String)
            mangaTitle = value
        End Set
    End Property

    Public Property Link As String
        Get
            Return mangaLink
        End Get
        Set(value As String)
            mangaLink = value
        End Set
    End Property

    Public Property ReadStat As String
        Get
            Return mangaReadStat
        End Get
        Set(value As String)
            mangaReadStat = value
        End Set
    End Property

    Public Property CompStat As String
        Get
            Return mangaCompStat
        End Get
        Set(value As String)
            mangaCompStat = value
        End Set
    End Property

    Public Property LocalPicLink As String
        Get
            Return mangaLocalPicLink
        End Get
        Set(value As String)
            mangaLocalPicLink = value
        End Set
    End Property

    Public Property Rating As Double
        Get
            Return mangaRating
        End Get
        Set(value As Double)
            mangaRating = value
        End Set
    End Property

    Public Property Genres As String
        Get
            Return mangaGenres
        End Get
        Set(value As String)
            mangaGenres = value
        End Set
    End Property

    Public Property Chapters As Integer
        Get
            Return mangaChapters
        End Get
        Set(value As Integer)
            mangaChapters = value
        End Set
    End Property

    Public Property TitleWords As String
        Get
            Return mangaTitleWords
        End Get
        Set(value As String)
            mangaTitleWords = value
        End Set
    End Property

End Class
