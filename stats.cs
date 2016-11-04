new ScriptObject(EleRPG_Stats) { };

new ScriptObject(EleRPG_StatTemplate)
{
	class = EleRPG_Stats;

	statLevel = 1;

	statStrength1 = 10;
	statStrength1_Rank = I;

	statEndurance1 = 10;
	statEndurance1_Rank = I;

	statDexterity1 = 10;
	statDexterity1_Rank = I;

	statAgility1 = 10;
	statAgility1_Rank = I;

	statMagic1 = 10;
	statMagic1_Rank = I;
	
	statLuck = 0;
};

function EleRPG_Stats::createNewStats(%this, %client)
{
	%client.EleRPG_stats = new ScriptObject(%client.bl_id @ "_stats" : EleRPG_StatTemplate);
}

function EleRPG_Stats::checkForStats(%this, %client)
{
	if(!isObject(%client.EleRPG_stats))
	{
		%this.createNewStats(%client);
		return false;
	} else
	{
		return true;
	}
}

function EleRPG_Stats::increaseStat(%this, %player, %stat)
{
	%player = %player.client;
	%obj = %player.EleRPG_stats;

	%x = %obj.stat[%stat @ %obj.statLevel] / 2;
	%y = 25 - (mPow(%x,2) / (60 * %x));
	%y = mCeil(%y / 3);

	%obj.stat[%stat @ %obj.statLevel] += %y;

	%this.setRank(%player, %stat);
}

function EleRPG_Stats::setRank(%this, %player, %stat)
{
	%player = %player.client;
	%obj = %player.EleRPG_stats;

	%rank = %this.detectRank(%obj.stat[%stat @ %obj.statLevel]);
	%obj.stat[%stat @ %obj.statLevel @ "_Rank"] = %rank;
}

function EleRPG_Stats::detectRank(%this, %stat)
{
	if(%stat < 100) {
		return "I";
	} else if(%stat < 200) {
		return "H";
	} else if(%stat < 300) {
		return "G";
	} else if(%stat < 400) {
		return "F";
	} else if(%stat < 500) {
		return "E";
	} else if(%stat < 600) {
		return "D";
	} else if(%stat < 700) {
		return "C";
	} else if(%stat < 800) {
		return "B";
	} else if(%stat < 900) {
		return "A";
	} else if(%stat < 1100) {
		return "S";
	} else if(%stat < 1300) {
		return "SS";
	} else if(%stat <= 1500) {
		return "SSS";
	}
}

function EleRPG_Stats::levelUp(%this, %player)
{
	%player = %player.client;
	%obj = %player.EleRPG_stats;

	if(%obj.statLevel == 50)
		return;

	%obj.statLevel++;

	%obj.statLuck = %obj.statLevel * 100;
	if(%obj.statLuck > 1500)
	{
		%obj.statLuck = 1500;
	}

	%obj.statLuck_Rank = %this.detectRank(%obj.statLuck);

	%obj.statStrength[%obj.statLevel] = 10;
	%obj.statStrength[%obj.statLevel @ "_Rank"] = I;

	%obj.statEndurance[%obj.statLevel] = 10;
	%obj.statEndurance1[%obj.statLevel @ "_Rank"] = I;

	%obj.statDexterity[%obj.statLevel] = 10;
	%obj.statDexterity[%obj.statLevel @ "_Rank"] = I;

	%obj.statAgility[%obj.statLevel] = 10;
	%obj.statAgility[%obj.statLevel @ "_Rank"] = I;

	%obj.statMagic[%obj.statLevel] = 10;
	%obj.statMagic[%obj.statLevel @ "_Rank"] = I;
}

function EleRPG_Stats::penalty(%this, %player)
{
	%player = %player.client;
	%obj = %player.EleRPG_stats;
	%obj.statLuck = %obj.statLuck - 100;

	%obj.statLuck_Rank = %this.detectRank(%obj.statLuck);

	// Clear current level of stats
	%obj.statStrength[%obj.statLevel] = "";
	%obj.statStrength1[%obj.statLevel @ "_Rank"] = "";

	%obj.statEndurance[%obj.statLevel] = "";
	%obj.statEndurance1[%obj.statLevel @ "_Rank"] = "";

	%obj.statDexterity[%obj.statLevel] = "";
	%obj.statDexterity1[%obj.statLevel @ "_Rank"] = "";

	%obj.statAgility[%obj.statLevel] = "";
	%obj.statAgility1[%obj.statLevel @ "_Rank"] = "";

	%obj.statMagic[%obj.statLevel] = "";
	%obj.statMagic1[%obj.statLevel @ "_Rank"] = "";

	// Decrease level
	%obj.statLevel--;

	// Reset previous level's stats
	%obj.statStrength[%obj.statLevel] = 10;
	%obj.statStrength1[%obj.statLevel @ "_Rank"] = I;

	%obj.statEndurance[%obj.statLevel] = 10;
	%obj.statEndurance1[%obj.statLevel @ "_Rank"] = I;

	%obj.statDexterity[%obj.statLevel] = 10;
	%obj.statDexterity1[%obj.statLevel @ "_Rank"] = I;

	%obj.statAgility[%obj.statLevel] = 10;
	%obj.statAgility1[%obj.statLevel @ "_Rank"] = I;

	%obj.statMagic[%obj.statLevel] = 10;
	%obj.statMagic1[%obj.statLevel @ "_Rank"] = I;
}
