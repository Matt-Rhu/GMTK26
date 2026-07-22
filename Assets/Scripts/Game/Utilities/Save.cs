using System;
using UnityEngine;
using System.IO;

public static class Save
{
    private const int saveVersion = 2;
    private const string saveDirectory = "/Save/";
    private const string saveExtenstion = ".sav";
    
    private static GeneralData general;
    private static LevelData level1;
    private static LevelData level2;
    private static LevelData level3;
    private static LevelData level4;
    
    public static GeneralData General
    {
        get { return general ??= InitGeneralData(); }
    }
    
    // public static LevelData CurrentLevel => Level(LevelLoader.instance.CurrentLevel);
    //
    //
    // public static LevelData Level(Levels level) => level switch
    // {
    //     Levels.LevelBlue => level1 ??= InitLevelData(level),
    //     Levels.LevelGreen => level2 ??= InitLevelData(level),
    //     Levels.LevelYellow => level3 ??= InitLevelData(level),
    //     Levels.LevelRed => level4 ??= InitLevelData(level),
    //     Levels.None => null,
    //     _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
    // };
    
    private static GeneralData InitGeneralData()
    {
        if (TryLoadFile(out GeneralData loadedData))
            return loadedData;
        return NewGeneralData();
    }

    // private static LevelData InitLevelData(Levels level)
    // {
    //     if (TryLoadFile(out LevelData loadedData, level))
    //         return loadedData;
    //     return NewLevelData();
    // }
    
    private static bool TryLoadFile<T>(out T loadedSave/*, Levels level = 0*/) where T : SaveData, new()
    {
        loadedSave = new T();
        var path = GetPath(typeof(T)/*, level*/);

        if (!File.Exists(path)) return false;
        
        using var reader = new StreamReader(path);
        var json = reader.ReadToEnd();
        
        loadedSave = JsonUtility.FromJson<T>(json);
        
        reader.Close();
        
        return true;
    }
    
    
    public static void WriteAll()
    {
        Write(general);
        // Write(level1, (Levels)1);
        // Write(level2, (Levels)2);
        // Write(level3, (Levels)3);
        // Write(level4, (Levels)4);
    }

    // public static void WriteCurrentLevel()
    // {
    //     WriteLevel(LevelLoader.instance.CurrentLevel);
    // }
    
    // public static void WriteLevel(Levels level)
    // {
    //     switch (level)
    //     {
    //         case Levels.LevelBlue:
    //             Write(level1, (Levels)1);
    //             break;
    //         case Levels.LevelGreen:
    //             Write(level2, (Levels)2);
    //             break;
    //         case Levels.LevelYellow:
    //             Write(level3, (Levels)3);
    //             break;
    //         case Levels.LevelRed:
    //             Write(level4, (Levels)4);
    //             break;
    //     }
    // }

    public static void WriteGeneral()
    {
        Write(general);
    }
    
    public static void Write(SaveData save/*, Levels level = 0*/)
    {
        if (save == null) return;
        
        string json = JsonUtility.ToJson(save);

        var path = GetPath(save.GetType()/*, level*/);
        
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        
        var fileStream = new FileStream(path, FileMode.Create);
        using var writer = new StreamWriter(fileStream);
        writer.Write(json);
        
        writer.Close();
    }
    
    
    private static string GetPath(Type saveType/*, Levels level = 0*/)
    {
        var fileName = saveType.Name;

        // if (saveType == typeof(LevelData))
        //     fileName += ((int)level).ToString();
        
        return Application.persistentDataPath + saveDirectory + fileName + saveExtenstion;
    }
    
    
    public static void OnQuit()
    {
        level1?.OnQuitReset();
        level2?.OnQuitReset();
        level3?.OnQuitReset();
        level4?.OnQuitReset();
        
        WriteAll();
    }
    
    private static GeneralData NewGeneralData()
    {
        var newData =  new GeneralData()
        {
            version = saveVersion,
         
            startedGameAlready = false,
            
            InvertedYAxis = false,
            HiddenHUD = false,
            RealTimeScore = false,
            
            TargetFrameRate = 60,
            language = Language.English,
            
            generalSoundSlider = 0.5f,
            musicSoundSlider = 1,
            SFXSoundSlider = 1,
        };

        return newData;
    }

    private static LevelData NewLevelData()
    {
        var newData =  new LevelData()
        {
            version = saveVersion
        };
        
        return newData;
    }
    
    // private static Language SteamLanguage()
    // {
    //     if (BuildSettings.ForSteam)
    //     {
    //         return SteamApps.GetCurrentGameLanguage() switch
    //         {
    //             "english" => Language.English,
    //             "french" => Language.French,
    //             _ => Language.English
    //         };
    //     }
    //     return Language.English;
    // }
}

public enum Language
{
    English,
    French
}


[Serializable]
public class SaveData
{
    public int version;
}

[Serializable]
public class GeneralData : SaveData
{
    public bool startedGameAlready;
    
    //settings
    public bool InvertedYAxis;
    public bool HiddenHUD;
    public bool RealTimeScore;
    
    public Language language;
    public int TargetFrameRate;
    
    public float generalSoundSlider;
    public float musicSoundSlider;
    public float SFXSoundSlider;
}

[Serializable]
public class LevelData : SaveData
{
    public int bombardierHighScore;
    public int injectorHighScore;
    public int riveteerHighScore;
    public int InterceptorHighScore;

    public float[] spawnOdds;
    public Vector3 deathPos;
    public int wins;
    public int runs;
    
    public void ResetDeathPos()
    {
        deathPos = Vector3.zero;
    }
    
    public void OnQuitReset()
    {
        ResetDeathPos();
#if !UNITY_EDITOR
        runs = 0;
#endif
    }
}