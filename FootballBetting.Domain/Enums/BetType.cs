namespace FootballBetting.Domain.Enums;

public enum BetType
{
    HomeWin = 1,
    AwayWin = 2,
    Draw = 3,
    
    // Goals/Shots
    Over05Goals = 10,
    Over15Goals = 11,
    Over25Goals = 12,
    Over35Goals = 13,
    Under05Goals = 14,
    Under15Goals = 15,
    Under25Goals = 16,
    Under35Goals = 17,
    
    // Corners
    Over65Corners = 20,
    Over75Corners = 21,
    Over85Corners = 22,
    Over95Corners = 23,
    Under65Corners = 24,
    Under75Corners = 25,
    Under85Corners = 26,
    Under95Corners = 27,
    
    // Cards
    Over15Cards = 30,
    Over25Cards = 31,
    Over35Cards = 32,
    Over45Cards = 33,
    Under15Cards = 34,
    Under25Cards = 35,
    Under35Cards = 36,
    Under45Cards = 37,
    
    // Shots
    Over85Shots = 40,
    Over95Shots = 41,
    Over105Shots = 42,
    Over115Shots = 43,
    Under85Shots = 44,
    Under95Shots = 45,
    Under105Shots = 46,
    Under115Shots = 47
}