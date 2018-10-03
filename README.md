# party-tower
current public build of party tower

You found the current build of Party Tower! There's a little bit of a downside right now, the current version is broken. 
There are a couple of errors going on in the background with our A* pathfinding which we're working on fixing. Essentially,
the code which sets up our custom data structure for the maps to be built just got an overhaul and since then we haven't 
had the time to put the A* functionality back in place.

If you're just interested in the parts that I've worked on, I've primarily been working in the LevelMapCoordinator class
and the external level designer tool. If you're looking to use the tool, the two tags which get exported don't always
mean what the tags say explicity, so be sure to check the documentation in the LevelMapCoordinator to know exatly what
the denotations mean. 

Our next steps are cleaning up Game1 and getting A* fully functional, at which point we'll be able to start cranking out levels.

Thanks for taking a look!
