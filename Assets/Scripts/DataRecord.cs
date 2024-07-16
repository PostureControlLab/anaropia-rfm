using CsvHelper.Configuration.Attributes;

class DataRecord
{
    [Index(0)] public float time { get; set; }

    [Index(1)] public float stim_pitch { get; set; }
    [Index(2)] public float stim_roll { get; set; }
    [Index(3)] public float stim_yaw { get; set; }
    [Index(4)] public float stim_trans_ap { get; set; }
    [Index(5)] public float stim_trans_ml { get; set; }
    [Index(6)] public float stim_trans_ud { get; set; }
    [Index(7)] public float stim_scale { get; set; }

    [Index(8)] public float xpos { get; set; }
    [Index(9)] public float ypos { get; set; }
    [Index(10)] public float zpos { get; set; }
    [Index(11)] public float xrot { get; set; }
    [Index(12)] public float yrot { get; set; }
    [Index(13)] public float zrot { get; set; }

    [Index(14)] public float shld_xpos { get; set; }
    [Index(15)] public float shld_ypos { get; set; }
    [Index(16)] public float shld_zpos { get; set; }
    [Index(17)] public float shld_xrot { get; set; }
    [Index(18)] public float shld_yrot { get; set; }
    [Index(19)] public float shld_zrot { get; set; }

    [Index(20)] public float hip_xpos { get; set; }
    [Index(21)] public float hip_ypos { get; set; }
    [Index(22)] public float hip_zpos { get; set; }
    [Index(23)] public float hip_xrot { get; set; }
    [Index(24)] public float hip_yrot { get; set; }
    [Index(25)] public float hip_zrot { get; set; }

    [Index(26)] public float lhand_xpos { get; set; }
    [Index(27)] public float lhand_ypos { get; set; }
    [Index(28)] public float lhand_zpos { get; set; }
    [Index(29)] public float lhand_xrot { get; set; }
    [Index(30)] public float lhand_yrot { get; set; }
    [Index(31)] public float lhand_zrot { get; set; }

    [Index(32)] public float rhand_xpos { get; set; }
    [Index(33)] public float rhand_ypos { get; set; }
    [Index(34)] public float rhand_zpos { get; set; }
    [Index(35)] public float rhand_xrot { get; set; }
    [Index(36)] public float rhand_yrot { get; set; }
    [Index(37)] public float rhand_zrot { get; set; }

    [Index(38)] public float lfoot_xpos { get; set; }
    [Index(39)] public float lfoot_ypos { get; set; }
    [Index(40)] public float lfoot_zpos { get; set; }
    [Index(41)] public float lfoot_xrot { get; set; }
    [Index(42)] public float lfoot_yrot { get; set; }
    [Index(43)] public float lfoot_zrot { get; set; }

    [Index(44)] public float rfoot_xpos { get; set; }
    [Index(45)] public float rfoot_ypos { get; set; }
    [Index(46)] public float rfoot_zpos { get; set; }
    [Index(47)] public float rfoot_xrot { get; set; }
    [Index(48)] public float rfoot_yrot { get; set; }
    [Index(49)] public float rfoot_zrot { get; set; }

    [Index(50)] public double analog0 { get; set; }
    [Index(51)] public double analog1 { get; set; }
    [Index(52)] public double analog2 { get; set; }
    [Index(53)] public double analog3 { get; set; }
    [Index(54)] public double analog4 { get; set; }
    [Index(55)] public double analog5 { get; set; }
    [Index(56)] public double analog6 { get; set; }
    [Index(57)] public double analog7 { get; set; }
    [Index(58)] public double analog8 { get; set; }
    [Index(59)] public double analog9 { get; set; }
    [Index(60)] public double analog10 { get; set; }
    [Index(61)] public double analog11 { get; set; }
    [Index(62)] public double analog12 { get; set; }
    [Index(63)] public double analog13 { get; set; }
}
