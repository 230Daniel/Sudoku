# Sudoku
A Sudoku solver in C# using the wave function collapse algorithm. I used NRTs for this project to see what all the fuss was about, I decided I'm not a fan.

## The Algorithm
The method of solving the sudoku is as follows:

 - Choose the tile with the least possible values
   - For each possible value
     - Set the tile to this value and propagate the information to the tiles in the same row, column, and segment - these tiles can no longer have this value.
     - If a tile complains that it must have the value it has been told it can't have, revert all changes and go to the next possible value
     - If all tiles were happy with the information, call the algorithm recursively
     - If the recursive result is true, we solved the sudoku. Return true.
     - If the recursive result is false, there was a problem with our change down the line. Revert all changes and go to the next possible value
   - If we went through all possible values, the tile cannot currently be assigned a valid value. Return false.

## Inspiration
Inspired by the video "[Superpositions, Sudoku, the Wave Function Collapse algorithm](https://www.youtube.com/watch?v=2SuvO4Gi7uY)" by [Martin Donald](https://www.youtube.com/channel/UC8bYucAICXmYet8pZ5Ja9Dw):
[![Superpositions, Sudoku, the Wave Function Collapse algorithm.](https://img.youtube.com/vi/2SuvO4Gi7uY/0.jpg)](https://www.youtube.com/watch?v=2SuvO4Gi7uY)
