
local table = {
	a = "10"
	subtable = {
		array = [1,2,3]
	},
	[10 + 123] = "expression index"
}

local array=[ 1, 2, 3, { a = 10, b = "string" } ];

foreach (i,val in array)
{
	::print("the type of val is" + typeof val);
}

foreach (var in collection_to_loop)
{
  try
  {
    local exception = 42;

  	throw exception;
  }
  catch (exception)
  {
    ::logInfo(exception);
  }
}
/////////////////////////////////////////////

class Entity
{	
	constructor(etype,entityname)
	{
		name = entityname;
		type = etype;
	}
	
	function kek()
	{
	   // Empty
	   local length = 0;

	   for (local i = 0; i < length; i++)
	   {
		 local a = 5;
		 local b = 6;
		 local c = 7;
	   }
	}

	x = 0;
	y = 0;
	z = 0;
	name = null;
	type = null;
}

function Entity::MoveTo(newx,newy,newz)
{
	x = newx;
	y = newy;
	z = newz;
}

class Player extends Entity {
	constructor(entityname)
	{
		base.constructor("Player",entityname)
	}

	function DoDomething()
	{
		::print("something");
	}
}

local newplayer = Player("da playar");
 
newplayer.MoveTo(100,200,300);	

local kek = 4;

local kek = 0;