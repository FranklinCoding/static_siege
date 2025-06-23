# static siege: Card Roguelike

Static Siege: Card Roguelike is a single-file, "pure" JavaScript game that blends reverse tower defense with deck-building mechanics. Defend your central Core against increasingly difficult waves of enemies by playing cards, managing a "Fuel" resource, and strategically upgrading your deck. This project is built with only HTML5 Canvas and vanilla JavaScript, no external game libraries needed.

How to Play
The Goal
Survive as many waves of enemies as you can. The game ends when your Core's health reaches zero.

Core Mechanics
The Core: Your base is at the center of the screen. It has a default Pulse Cannon that fires automatically at the nearest enemy. Its health is represented by the cyan ring around it.

Fuel: The magenta bar on the left is your Fuel. Fuel is required to play cards. You start with 5 Fuel and gain 1 more for every 100 points you earn by destroying enemies.

Cards: Your actions are driven by the cards in your hand at the bottom of the screen.

Playing Cards: Click a card in your hand to play it. This consumes Fuel equal to the cost shown on the card.

Drawing Cards: You start with 4 cards and draw 2 more at the start of each new wave. If your deck runs out, your discard pile is shuffled to create a new one.

Hand Limit: You can hold a maximum of 7 cards.

Scrap & The Shop:

Destroying enemies earns you Scrap.

Every 5th wave, a Shop will appear. Here you can spend your Scrap to buy powerful new cards to add to your deck.

Card Types
Attack (Red): Cards that directly damage enemies or boost your weapons.

Defense (Blue): Cards that heal your Core or provide shields.

Utility (Green): Cards that manipulate your resources, like Fuel or your hand of cards.

Permanent (Yellow): Powerful, one-time use upgrades that are removed from your deck after being played.

How to Run
Since this entire game is contained within a single index.html file, running it is very simple:

Download the Code: Clone this repository or download the index.html file to your local machine.

Open in Browser: Open the index.html file in any modern web browser (like Google Chrome, Mozilla Firefox, or Microsoft Edge).

That's it! The game will start immediately.
