
clear all; close all

for tr = 1:9

    t=(1:20000)'/1000;
    f1=1;
    
    switch tr
        case 1
            a_pitch = 10; a_roll = 0; a_yaw = 0;
            a_trans_ap = 0; a_trans_ml = 0; a_trans_ud = 0;
            a_scale = 0;
        case 2
            a_pitch = 0; a_roll = 10; a_yaw = 0;
            a_trans_ap = 0; a_trans_ml = 0; a_trans_ud = 0;
            a_scale = 0;
        case 3
            a_pitch = 0; a_roll = 0; a_yaw = 10;
            a_trans_ap = 0; a_trans_ml = 0; a_trans_ud = 0;
            a_scale = 0;
        case 4
            a_pitch = 0; a_roll = 0; a_yaw = 0;
            a_trans_ap = 0.1; a_trans_ml = 0; a_trans_ud = 0;
            a_scale = 0;
        case 5
            a_pitch = 0; a_roll = 0; a_yaw = 0;
            a_trans_ap = 0; a_trans_ml = 0.1; a_trans_ud = 0;
            a_scale = 0;
        case 6
            a_pitch = 0; a_roll = 0; a_yaw = 0;
            a_trans_ap = 0; a_trans_ml = 0; a_trans_ud = 0.1;
            a_scale = 0;
        case 7
            a_pitch = 0; a_roll = 0; a_yaw = 0;
            a_trans_ap = 0; a_trans_ml = 0; a_trans_ud = 0;
            a_scale = 1;
        case 8
            a_pitch = 10; a_roll = 0; a_yaw = 0;
            a_trans_ap = 0.1; a_trans_ml = 0; a_trans_ud = 0;
            a_scale = 0;
        case 9
            a_pitch = 0; a_roll = 10; a_yaw = 0;
            a_trans_ap = 0.2; a_trans_ml = 0; a_trans_ud = 0;
            a_scale = 1;
    end        
        
        
    pitch = a_pitch * sin(2*pi*f1 * t);
    roll =  a_roll * sin(2*pi*f1 * t);
    yaw =   a_yaw * sin(2*pi*f1 * t);
    trans_ap = a_trans_ap * sin(2*pi*f1 * t);
    trans_ml = a_trans_ml * sin(2*pi*f1 * t);
    trans_ud = a_trans_ud * sin(2*pi*f1 * t);
    if a_scale==0
        scale = ones(size(t));
    else
        scale = 1.5 + a_scale * sin(2*pi*f1 * t);
    end
    
    figure
    subplot(311)
        plot(t,[pitch, roll, yaw])
    subplot(312)
        plot(t,[trans_ap, trans_ml, trans_ud])
    subplot(313)
        plot(t,scale)

    % %%write csv
    % % out = [{'time'},{'trans_ap'},{'trans_ml'}; num2cell([t',stim_ap',stim_ml'])];
    % % writematrix(out,['vr_stim_' num2str(k) '.csv'])
    dlmwrite(['vr_stim_' num2str(tr) '.csv'],'time,pitch,roll,yaw,trans_ap,trans_ml,trans_ud,scale','')
    dlmwrite(['vr_stim_' num2str(tr) '.csv'],num2cell([t,pitch,roll,yaw,trans_ap,trans_ml,trans_ud,scale]),'-append','delimiter',',')
end
