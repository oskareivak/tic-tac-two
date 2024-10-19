// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design;
using DAL;
using GameBrain;
using MenuSystem;


var configRepository = new ConfigRepository();

var deepMenu = new Menu(
    EMenuLevel.Deep,
    "TIC-TAC-TOE DEEP", menuItems: [
        new MenuItem()
        {
            Shortcut = "Y",
            Title = "YYYYY",
            MenuItemAction = null
        },
]);

var optionsMenu = new Menu(
    EMenuLevel.Secondary,
    "TIC-TAC-TOE Options", menuItems: [
    new MenuItem()
    {
        Shortcut = "X",
        Title = "X Starts",
        MenuItemAction = deepMenu.Run
    },
    new MenuItem()
    {
        Shortcut = "O",
        Title = "O Starts",
        MenuItemAction = null
    }
]);

var mainMenu = new Menu(
    EMenuLevel.Main,
    "TIC-TAC-TOE", menuItems: [
    new MenuItem()
    {
        Shortcut = "O",
        Title = "Options",
        MenuItemAction = optionsMenu.Run
    },
    new MenuItem()
    {
        Shortcut = "N",
        Title = "New game",
        MenuItemAction = NewGame
    }
]);

mainMenu.Run();

string NewGame()
{   
    // TODO: choose configuration
    var configMenuItems = new List<MenuItem>();

    for (int i = 0; i < configRepository.GetConfigurationNames().Count; i++)
    {
        var returnValue = i.ToString();
        configMenuItems.Add(new MenuItem()
        {
            Title = configRepository.GetConfigurationNames()[i],
            Shortcut = (i+1).ToString(),
            MenuItemAction = () => returnValue
        });
    }
    
    var configMenu = new Menu(EMenuLevel.Deep,
        "TIC-TAC-TWO - choose game config",
        configMenuItems
        );

    var chosenConfigShortcut = configMenu.Run();
    
    if (!int.TryParse(chosenConfigShortcut, out var configNo))
    {
        return chosenConfigShortcut;
    }

    var chosenConfig = configRepository.GetConfigurationByName(
        configRepository.GetConfigurationNames()[configNo]
    );
    
    var gameInstance = new TicTacTwoBrain(chosenConfig);

    ConsoleUI.Visualizer.DrawBoard(gameInstance);
         
    Console.Write("Give me coordinates <x,y>:");
    var input = Console.ReadLine()!;
    var inputSplit = input.Split(",");
    var inputX = int.Parse(inputSplit[0]);
    var inputY = int.Parse(inputSplit[1]);
    gameInstance.MakeAMove(inputX, inputY);
    
    // valideeri et seal on yldse koma, siis et on ainult yks koma(kas seda vaja?), split string and TRY parse, ja siis
    // validate coordinates that they actually fit on the board, is the piece there that you actually can make a move vms
    
    // loop
    // draw the board again
    // ask input again, validate (again)
    // is the game over?
    
    
    return "";
}

// TODO: Gameboard ja grid saaks eraldada kasutades eri v2rve v ss symboleid.
// MÕTE: Peab kuidagi saama user inputi, et kuhu tema m2rk maha panna. Kas votta console.readkey-ga, nii et saab 
//       boardi peal ringi liikuda? Lihtsaim viis on muidugi lis koordinaate kysida (siis vaja palju checke teha). 
//       Kui teen koordinaatidega siis tasuks boardiga koos selle korvale valja printida ka koordinaatide nr, et oleks 
//       userile selgem, ja siis ka et mis pidi X ja mis pidi Y.

// should the user be able to edit built in configurations? up to me! Ma arvan, et ei peaks saama.
// voiks olla reset nupp ehk kui oled liiga palju confe teinud vms, siis saad tagasi resettida koik defaulti. v lis kustutada enda confe.

// Reegleid peaks saama confida ka nii, et saab mangida tavalist tic-tac-toe-d, ehk 3x3 board ja grid ja 5+5 nuppu.

// voimalikult vahe hard coded asju, ehk voimalikult palju lahtist ja confitavat

// esimese kodutoo jaoks pole vaja state savingut vms

// boardi jms settinguid saab satestada eraldi - ehk player saab teha endale erinevad vaikesatted valmis optionsitest, 
//   kus tal kusitakse kui suur board, kui suur grid ja koik muu ja mangu minnes kysitakse ainult et milliste valmiss2tetega  m2ngu minna
// Menyy voiks ka arrowkeydega teha, mitte t2htede sisestamisega.

//
// MenuMain();
// var choice:string? = Console.ReadLine();
//
// if (choice == 0)
// {
//     
// }
//
// return;
//
//
// static void MenuMain()
// {
//     MenuStart();
//     
//     Console.Clear();
//     Console.WriteLine("TIC-TAC-TOE");
//     Console.WriteLine("=============================");
//     Console.WriteLine("O) Options");
//     Console.WriteLine("N) New game");
//     Console.WriteLine("L) Load game");
//     Console.WriteLine("E) Exit");
//     MenuEnd();
// }
//
// static void MenuOptions()
// {
//     
// }
//
// static void MenuEnd()
// {
//     
// }