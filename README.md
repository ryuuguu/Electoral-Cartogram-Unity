# Electoral-Cartogram-Unity
Unity clone of Electoral Cartogram including map editor.

Currently working on changing UI to speed up initial load.

Version Scenes
  Basic Canada - URL
    First try, pretty slow in in making map and switching maps
  Square Canada - URL
    Same as basic but switched to offset squares instead of hexes.
  UIEHexGridCanada - URL
    Same features as Basic Canada
    uses new HexGrid - refactored to separate display from hex coordinate management
    UI refactored to remove instantiating unneeded GameObjects
      still slow but much faster

Future directions of code

testing hexes that are
   GameObjects with Mesh
      - not difficult with HexGrid, but not much to be gained
   DOTS entities with mesh
      - major changes as HexGrid does not support dots
      - DOTS version of HexGrid will be major refactoring but I want to learn DOTS so this is next target
      - DOTS should be much faster
   UIElements VisualElements with image (waiting till 2020.1.xb is out)
      - UIELements won't be in Beta for runtime till 2020.1 so waiting
      - UIELements of HexGrid works in editor now.
      - might be much faster than basic
