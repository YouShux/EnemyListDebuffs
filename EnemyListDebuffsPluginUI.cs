using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Dalamud.Interface.Components;

namespace EnemyListDebuffs
{
    public class EnemyListDebuffsPluginUI : IDisposable
    {
        private readonly EnemyListDebuffsPlugin _plugin;

#if DEBUG
        private bool ConfigOpen = true;
#else
        private bool ConfigOpen = false;
#endif
        public bool IsConfigOpen => ConfigOpen;

        public EnemyListDebuffsPluginUI(EnemyListDebuffsPlugin p)
        {
            _plugin = p;

            _plugin.Interface.UiBuilder.OpenConfigUi += UiBuilder_OnOpenConfigUi;
            _plugin.Interface.UiBuilder.Draw += UiBuilder_OnBuild;
        }

        public void Dispose()
        {
            _plugin.Interface.UiBuilder.OpenConfigUi -= UiBuilder_OnOpenConfigUi;
            _plugin.Interface.UiBuilder.Draw -= UiBuilder_OnBuild;
        }

        public void ToggleConfig()
        {
            ConfigOpen = !ConfigOpen;
        }

        public void UiBuilder_OnOpenConfigUi() => ConfigOpen = true;

        public void UiBuilder_OnBuild()
        {
            if (!ConfigOpen)
                return;

            ImGui.SetNextWindowSize(new Vector2(420, 647), ImGuiCond.FirstUseEver);

            // 窗口标题汉化，不影响插件名称与逻辑
            if (!ImGui.Begin("敌方列表减益", ref ConfigOpen, ImGuiWindowFlags.None))
            {
                ImGui.End();
                return;
            }

            bool needSave = false;

            if (ImGui.CollapsingHeader("通用", ImGuiTreeNodeFlags.DefaultOpen))
            {
                needSave |= ImGui.Checkbox("启用", ref _plugin.Config.Enabled);
                needSave |= ImGui.InputInt("更新间隔(毫秒)", ref _plugin.Config.UpdateInterval, 10);
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("状态刷新间隔（毫秒）");
                if (ImGui.Button("重置配置为默认值"))
                {
                    _plugin.Config.SetDefaults();
                    needSave = true;
                }
                ImGui.Text("打开设置界面时会显示测试节点以便调整参数。");
            }

            if (ImGui.CollapsingHeader("节点组", ImGuiTreeNodeFlags.DefaultOpen))
            {
                needSave |= ImGui.Checkbox("从右向左填充", ref _plugin.Config.FillFromRight);
                needSave |= ImGui.SliderInt("X偏移", ref _plugin.Config.GroupX, -200, 200);
                needSave |= ImGui.SliderInt("Y偏移", ref _plugin.Config.GroupY, -200, 200);
                needSave |= ImGui.SliderInt("节点间距", ref _plugin.Config.NodeSpacing, -5, 30);
                needSave |= ImGui.SliderFloat("整体缩放", ref _plugin.Config.Scale, 0.01F, 3.0F);
            }

            if (ImGui.CollapsingHeader("节点", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Text("图标宽高建议保持 3:4 比例以获得最佳效果");
                needSave |= ImGui.SliderInt("图标 X 偏移", ref _plugin.Config.IconX, -200, 200);
                needSave |= ImGui.SliderInt("图标 Y 偏移", ref _plugin.Config.IconY, -200, 200);
                needSave |= ImGui.SliderInt("图标宽度", ref _plugin.Config.IconWidth, 5, 100);
                needSave |= ImGui.SliderInt("图标高度", ref _plugin.Config.IconHeight, 5, 100);
                needSave |= ImGui.SliderInt("持续时间 X 偏移", ref _plugin.Config.DurationX, -200, 200);
                needSave |= ImGui.SliderInt("持续时间 Y 偏移", ref _plugin.Config.DurationY, -200, 200);
                needSave |= ImGui.SliderInt("持续时间字体大小", ref _plugin.Config.FontSize, 1, 60);
                needSave |= ImGui.SliderInt("持续时间内边距", ref _plugin.Config.DurationPadding, -100, 100);

                needSave |= ImGui.ColorEdit4("持续时间文本颜色", ref _plugin.Config.DurationTextColor);
                needSave |= ImGui.ColorEdit4("持续时间描边颜色", ref _plugin.Config.DurationEdgeColor);
            }

            if (needSave)
            {
                _plugin.StatusNodeManager.LoadConfig();
                _plugin.Config.Save();
            }

            ImGui.End();
        }
    }
}
