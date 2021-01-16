# Electoral Cartogram Unity

Created by [Grant Morgan](https://github/ryuuguu) under an [MIT License](https://github.com/ryuuguu/Electoral-Cartogram-Unity/blob/master/LICENSE). Copyright &copy; 2020 Grant Morgan.

This site interactively shows federal election results for Canada 2019 in a [cartogram](https://en.wikipedia.org/wiki/Cartogram) that emphasizes population distribution by presenting every electoral district as the same shape and size. It also can show the percentage of the vote for each party per riding graphically.
You can see  a link published on my home page [Ryuuguu.com](https://ryuuguu.com).

The original design and map ( and readme ) were copied from [Electoralcartogram](https://raw.githubusercontent.com/attaboy/electoralcartogram/) and built with a custom editor map builder included in the project. The app is written in Unity C# using a UI Toolkit 1.0 preview ( 13 is the last tested version.) It runs on standalone (Windows, Mac, Linux) Mobile & desKtop web browsers. It does not run on mobile browsers. Text features are limited in this preview so code for nice looking text is not implemented awaiting later releases of UI Toolkit. 

This project was done to stress test the UI Toolkit and so it draws 33k hexes each time the map, with each party's proportion of the vote, is drawn. A production version would draw this in the background and make an image of it, so switching would be fast. The map editor is not user friendly and there no instructions. All images are drawn from data at runtime.

Election results for [recent](https://enr.elections.ca/National.aspx?lang=e) and [past elections](https://elections.ca/content.aspx?section=ele&dir=pas&document=index&lang=e) were copied from [Elections Canada](https://elections.ca) under their terms and conditions for non-commercial reproduction of data.

## Development

Tested on Unity 2020.2.0f1 with
	UI Toolkit 1.0 preview 13
	UI builder 1.0 preview 11