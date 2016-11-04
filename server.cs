// ---
// Core functions and stuff
// ---

function serverCmdreload(%c, %file)
{
	if(!%c.isSuperAdmin)
		return;

	ChatMessageAll('',"<font:consolas:18>\c7[RPG] \c6Reloading files...");
	exec("./server.cs");
}

function serverCmdexec(%c, %file)
{
	if(!%c.isSuperAdmin)
		return;

	ChatMessageAll('', "<font:consolas:18>\c7[RPG] \c6Reloading file...");
	exec("./" @ %file @".cs");
}

function serverCmdRPG(%client)
{
	%client.chatMessage("<font:consolas:18>\c6---"); // \c0 red, \c3 yellow, \c2 green
	%client.chatMessage("<font:consolas:18>\c6Current RPG features:");
	%client.chatMessage("<font:consolas:18>\c6Only worked on features are listed. The \c3*\c6 item in the list is being worked on now.");
	%client.chatMessage("<font:consolas:18>\c6Progress is shown by colours. Red means little to no progress, green means almost complete.");
	%client.chatMessage("<font:consolas:18>\c6 * \c0Player Abilities");
	%client.chatMessage("<font:consolas:18>\c6 1 \c0Damage System");
}

function ServerCmdToggleRPG(%client)
{
	if(%client.isSuperAdmin)
	{
		if($Server::RPG::Running)
		{
			$Server::RPG::Running = false;
			ChatMessageAll('',"<font:consolas:18>\c7[RPG] \c6Toggled RPG off.");
		}
		else
		{
			$Server::RPG::Running = true;
			ChatMessageAll('',"<font:consolas:18>\c7[RPG] \c6Toggled RPG on.");
		}
	}
}

function fcbn(%n)
{
	return findClientByName(%n);
}

$e = fcbn(Element);

// ---
// Execs
// ---

exec("./damage.cs");
exec("./stats.cs");
