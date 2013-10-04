Dominion
========

A dominion simulator for playing the card game of dominion.

The goals of the project are as follows, in priority order.

  1) Create a game engine which enforces the rules of Dominoin the game.
     Separation of Game Rules from AI or heuristics is required.  
  2) Simulation of all available dominion cards.
  3) Ability to easily extend the game engine with custom cards.
  4) Include a foundation for easily composing strategies from simple build orders.
  5) Include default strategies for playing most cards. 
  5) Create a strategy optimizer that given a set of 10 cards, finds the best combination to play.
  
Goal 5 is lofty - but would be very nice :).

Setup:   
======

  1) Install Visual Studio express desktop 2012.  You only need C#.   
  2) Load Dominoin.sln from the root of the repository
  3) Set a breakpoint at the end of main in Program.cs   (You dont want to lose the output)
  3) Run the "TestProgram" from the ide.  
  
What Does the Test Program do?
==============================

The simulator does 2 primary things right now.  

1) It compares 2 strategies against each other, and tells you which one is better.  
   The ComparePlayers method does this.

2) It compares a specific strategy against all other known strategies.  
   Use CompareStrategyVsAllKnownStrategies for this.

You will have to tweak the  main program to get it to do one of the above.  There is no UI for it

Where Is The Output for the Program?
=====================================

All output is to the console right now.  Set a break point at end of main so you dont lose
the output under the debugger.

You can also find invidual log files for examample games under TestProgram/Results.   These
log files are crucial when debugging the new strategy.

What does a stratgy Look like?
===============================

You can write a strategy to behave any way you want, but that's a lot of work.  

Most strategies right now derive from PlayerAction.  You specify a purchase order, discard order, 
trash order etc... along with a few method overrides and voila!

There are many example strategies included already.  Look in the TestProgram/Strategies folder. 

How Do I Contribute?
====================

I am the only ones making checkins right now, but please send me a changelist of anything you would like and 
I will try to get it included.

Though the project is public, please refrain from forking the code.  When I'm making changes, 
there will be large amounts of refactoring and re-organization.  I will keep everything checked in working, but
will not be worred about breaking external dependencies.

What Should I Contribute?
=========================

I have decided to begin collaboration on this project before it is complete.  It's already useful enough
for people to play around with it.  Expect to find bugs.

1) Many of the cards are implemented, but not all of them.  Goal is to eventually have them all complete.
2) Write innovating strategies.  See how yours does on the leader board
3) We need test case infrastucture.  Long term, I would like to see a test case for every clarifiction in the rule book.  
4) Contribute to the AI portion of the project.  

Your Program Crashed or threw an exception.  It must be a piece of crap right?
==============================================================================

The game engine enforces the rules of the game.  If a strategy is breaking the rules, it will throw an exception.
The correct fix here is to fix the strategy.

The game will also throw an excpetion if you use a card that hasn't been implemented yet.

There are also bugs ...




