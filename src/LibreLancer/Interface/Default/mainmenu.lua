-- NOTE: This is reinited on scene change. Don't try and keep things in here between changes
local MainMenu = {}

-- Entry Animation

-- Exit Animation
function MainMenu.ExitAnimation(f)
	GetElement('exit'):Animate('flyoutleft', 0, 0.6)
	GetElement('options'):Animate('flyoutleft', 0.05, 0.6)
	GetElement('multiplayer'):Animate('flyoutleft', 0.1, 0.6)
	GetElement('loadgame'):Animate('flyoutleft', 0.15, 0.6)
	GetElement('newgame'):Animate('flyoutleft', 0.2, 0.6)
	Timer(0.8, f)
end

function MainMenu.Init()
	-- Entry Animation
	GetElement('newgame'):Animate('flyinleft', 0, 0.6)
	GetElement('loadgame'):Animate('flyinleft', 0.05, 0.6)
	GetElement('multiplayer'):Animate('flyinleft', 0.1, 0.6)
	GetElement('options'):Animate('flyinleft', 0.15, 0.6)
	GetElement('exit'):Animate('flyinleft', 0.2, 0.6)
	-- Events
	GetElement('newgame').Clicked:Add(function ()
		MainMenu.ExitAnimation(function ()
			Game:NewGame()
		end)
	end)
	GetElement('loadgame').Clicked:Add(function ()

	end)
	GetElement('multiplayer').Clicked:Add(function ()
		MainMenu.ExitAnimation(function ()
			SwitchTo('multiplayer')
		end)
	end)
	GetElement('options').Clicked:Add(function ()
		MainMenu.ExitAnimation(function ()
			SwitchTo('options')
		end)
	end)
	GetElement('exit').Clicked:Add(function ()
		MainMenu.ExitAnimation(function ()
			Game:Exit()
		end)
	end)

end

local Multiplayer = {}

function Multiplayer.Init()
	GetElement('mainmenu').Clicked:Add(function()
		Multiplayer.ExitAnimation(function ()
		    Game:StopNetworking()
		    SwitchTo('mainmenu')
		end)
	end)
	GetElement('listtable'):SetData(Game:ServerList())
	ConnectButton = GetElement('connect')
	ConnectButton.Clicked:Add(function()
	    Game:ConnectSelection()
	end)
	DescriptionBlock = GetElement('descriptiontext')
	GetElement('animgroupA'):Animate('flyinleft', 0, 0.8)
    GetElement('animgroupB'):Animate('flyinright', 0, 0.8)
    Game:StartNetworking()
end

function Multiplayer.ExitAnimation(f)
    GetElement('animgroupA'):Animate('flyoutleft', 0, 0.8)
    GetElement('animgroupB'):Animate('flyoutright', 0, 0.8)    
    Timer(0.8, f)
end

function Multiplayer.Update()
    local sv = Game:ServerList()
    ConnectButton.Enabled = sv:ValidSelection()
    DescriptionBlock.Text = sv:CurrentDescription()
end

function Multiplayer.CharacterList()
    Multiplayer.ExitAnimation(function ()
        SwitchTo('characterlist')
    end)
end

local CharacterList = {}

function CharacterList.Init()

end

local Options = {}

function Options.Init()
	-- Entry Animation
	GetElement('general'):Animate('flyinleft', 0, 0.6)
	GetElement('controls'):Animate('flyinleft', 0.05, 0.6)
	GetElement('performance'):Animate('flyinleft', 0.1, 0.6)
	GetElement('audio'):Animate('flyinleft', 0.15, 0.6)
	GetElement('credits'):Animate('flyinleft', 0.2, 0.6)
	GetElement('return'):Animate('flyinleft', 0.25, 0.6)
	-- Left Buttons
	GetElement('return').Clicked:Add(function()
		SwitchTo('mainmenu')
	end)

end

local Scenes = {
	mainmenu = MainMenu,
	options = Options,
	multiplayer = Multiplayer,
	characterlist = CharacterList
}

CurrentScene = Scenes[SceneID()]
CurrentScene.Init()

function SceneEvent(ev)
    Events[ev] = function ()
        if CurrentScene[ev] ~= nil then
            CurrentScene[ev]()
        end
    end
end

SceneEvent('Update')
SceneEvent('CharacterList')
