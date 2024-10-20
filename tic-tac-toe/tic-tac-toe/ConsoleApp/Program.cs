using ConsoleApp;

Menus.MainMenu.Run();


// TODO: Gameboard ja grid saaks eraldada kasutades eri v2rve v ss symboleid.
// MÕTE: Peab kuidagi saama user inputi, et kuhu tema m2rk maha panna. Kas votta console.readkey-ga, nii et saab 
//       boardi peal ringi liikuda? Lihtsaim viis on muidugi lis koordinaate kysida (siis vaja palju checke teha). 
//       Kui teen koordinaatidega siis tasuks boardiga koos selle korvale valja printida ka koordinaatide nr, et oleks 
//       userile selgem, ja siis ka et mis pidi X ja mis pidi Y.

// should the user be able to edit built in configurations? up to me! Ma arvan, et ei peaks saama.
// voiks olla reset nupp ehk kui oled liiga palju confe teinud vms, siis saad tagasi resettida koik defaulti. v lis kustutada enda confe.

// DONE: Reegleid peaks saama confida ka nii, et saab mangida tavalist tic-tac-toe-d, ehk 3x3 board ja grid ja 5+5 nuppu.

// voimalikult vahe hard coded asju, ehk voimalikult palju lahtist ja confitavat

// esimese kodutoo jaoks pole vaja state savingut vms

// boardi jms settinguid saab satestada eraldi - ehk player saab teha endale erinevad vaikesatted valmis optionsitest, 
//   kus tal kusitakse kui suur board, kui suur grid ja koik muu ja mangu minnes kysitakse ainult et milliste valmiss2tetega  m2ngu minna
// Menyy voiks ka arrowkeydega teha, mitte t2htede sisestamisega.

// teha vb conf valik, et kas saab ainult liigutada piece mis on gridi sees v kogu boardi ulatuses olevaid piece-sid