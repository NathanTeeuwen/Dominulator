Dominulator
===========

A dominion simulator for playing the card game of dominion.  It now includes a front end UI.   The UI is a card picking UI that will help you setup a game of dominion.   Unlike most card pickers, you have the ability to run simulations of simple card combinations.  A report will be generated which gives insights that can guide you into how to play this set of cards

![Alt text](https://github.com/NathanTeeuwen/Dominulator/blob/master/Documentation/Win8Client.png "Windows 8 Dominulator front end")

The goals of the project are as follows.

 1. Create a game engine which enforces the rules of Dominion the game.
     Separation of Game Rules from AI or heuristics is required.  
 2. Simulation of all available dominion cards.
 3. Ability to easily extend the game engine with custom cards.
 4. Include a foundation for easily composing strategies from simple build orders.
 5. Include default strategies for playing most cards. 
 6. Create a strategy optimizer that given a set of 10 cards, finds the best combination to play.
 7. Includes ability to generate a standalone HTML report, detailing the play of the strategies
 8. A windows 8 UI for composing strategies

  
Goal 6 is lofty - but would be very nice :).

Setup
=====

 1. Install Visual Studio express desktop 2013.  You only need C#.   
 2. Load Dominion.sln from the root of the repository
 3. Make sure a Results folder exists in the root of your enlistment
 4. In the TestPrograms folder, select one of the test programs for startup.

OR to contribute to the UI

 1. Install Visual Studio express 2013.   
 2. Load Win8Client.sln from the root of the repository

Module Structure
=================

Dominoin
---------
Contains the logic of the game and all card definitions.  Enforces the rules of the game.   
Exposes the IPlayerAction interface that must be implemented to create player behavior.
Also contains human readable game log output and debug log output.

Dominion.Strategy
-----------------
Contains a framework for defining dominion strategies.  
PlayerAction can be customized with a purchase order, action order, gain order, trash order, discard order etc ...  This emulates
the strategy definition patterns seen in other common simulators.
PlayerAction also has a set of default play rules for each card.   There are many more default play rules that need to be defined

Dominion.DataGathering
----------------------
Contains an implementaiton of the GameLog that gathers various statistics about the game.  These statistics are used to create pretty 
graphs in the html renderer and in the webservice

Dominion.BuiltInStrategies
--------------------------
Lots of examples of built in strategies.  All of these strategies demonstrate how to use the strategy framework defined in 
Dominion.Strategy

Dominion.DynamicStrategyLoader
------------------------------
Contains a copy of all the strategies in Dominion.BuiltInStrategy.   In this module, they are compiled as embedded resources. 
The module includes methods to dynamically compile and load strategies at run time.

Dominion.StrategyOptimizer
---------------------------
Some rudimentary code that searches for the best set of parameters in a given strategy to optimize the win rate vs a control strategy

WebService
----------
An exe that launches a local webservice exposing most of the functionality of the simulator.   The dynamic strategy loader
allows the user to type in new strategies into the web app - which can then be compared against one another.

HtmlRenderer
------------
Code for generating Html Reports

Resources
---------
Misc helper methods for working with resources

TestOutput
----------
Common methods used by the various test programs.
  
What do the various test programs do?
=====================================

There are several different test programs that you can use to test the functionality of the simulator.
These can all be found in the TestPrograms folder

ComparePlayers.exe
----------------------    
   It compares 2 strategies against each other, and tells you which one is better.   There are lots of example strategies to 
   try in the Strategies namespace.   Modify the program to use one of the different built in strategies, or create your own.  
   There are various verbosity options on the compare method for seeing the results.  Setting CreateHtmlReport to true 
   writes out a detailed html file with graphs and gamelogs.  There are also code for various other simulations in this project
   that I haven't factored out yet

TestAllCardsWithBigMoney.exe
----------------------------------
   This is a regression test that will test all of the existing cards with their default strategies against a bigmoney strategy.

TestCompareStrategyVsAllKNownStrategies.exe
----------------------------------------------------
   When you write a new strategy, this program will rank it against all of the built in strategies that have been checked in so far.
   As checked in, it compares all of the strategies to bigmoney.   Change bigmoney to someother strategy to see how yours compares

TestStrategyOptimizer.exe
-----------------------------
   The strategy optimizer allows strategies to be parameterized.  Use this program for a relatively simple genetic algorithm that will
   find the best parameters.  A work in progress still.

You can also set the Default Startup project to the Webservice project.  

WebService
-------------
The webservice is a playground I have been using.  Once launched, browse to http://localhost:8081/dominion    You will see a webapp
that is capable of dynamically loading all included strategies (in the dynamic strategy loader module).  You can edit 
the strategies in the
web browser and get some quick reports on how they compare.

Win8Client
-------------
This folder contains the UI of the project.

Where Is The Output for the Program?
=====================================

Test programs output to the console.   They will also output files in the Results directory of the root of the git repository.

Most test programs can be configured to output individual game logs and also an html summary report.  

The Webservice provides a more interactive view of the output

What Does a Strategy Look Like?
===============================

You can write a strategy to behave any way you want, but that's a lot of work.  

Most strategies right now derive from PlayerAction.  You specify a purchase order, discard order, 
trash order etc... along with a few method overrides and voila!

There are many example strategies included already.  Look in the Dominion.BuiltInStrategies project. 

How Do I Contribute?
====================

I am the only ones making checkins right now, but please send me a changelist of anything you would like and 
I will try to get it included.

Though the project is public, please refrain from forking the code.  When I'm making changes, 
there will be large amounts of refactoring and re-organization.  I will keep everything checked in working, but
will not be worried about breaking external dependencies.

What Should I Contribute?
=========================

I have decided to begin collaboration on this project before it is complete.  It's already useful enough
for people to play around with it.  Expect to find bugs.

 1. Many of the cards are implemented, but not all of them.  Goal is to eventually have them all complete.
 2. Implement default strategies for the cards so they play as well as possible without custom strategies
 3. Write innovating strategies.  Compare your strategy vs the others
 4. We need test case infrastructure.  Long term, I would like to see a test case for every clarifiction in the rule book.
 5. Contribute to the strategy optimizer

Dominion has 205 kingdom cards.  There are currently 31 cards that can throw NotImplementedException.  

Your Program Crashed or Threw an Exception.  Why?
=================================================

The game engine enforces the rules of the game.  If a strategy is breaking the rules, the game engine will throw an exception.   The correct fix here is to fix the strategy.

If you are implementing a player action and forget to implement a callback that a card needs, you will get an exception.

The game will also throw an exception if you use a card that hasn't been implemented yet.

There are also bugs ...




