using CitizenFX.Core.Native;

namespace Client.Handlers;

public class MinimapHandler{
    /*
     * Returns a Minimap object with the following details:
     * x, y: Top left origin of minimap
     * width, height: Size of minimap (not pixels!)
     * left_x, right_x: Left and right side of minimap on x axis
     * top_y, bottom_y: Top and bottom side of minimap on y axis
     */
    public static Minimap GetMinimapAnchor(){
        float safeZone = API.GetSafeZoneSize();
        const float safeZone_x = 1.0f / 20.0f;
        const float safeZone_y = 1.0f / 20.0f;
        float aspect_ratio = API.GetAspectRatio(false);
        int res_x = 0, res_y = 0;
        API.GetActiveScreenResolution(ref res_x, ref res_y);
        float xScale = 1.0f / res_x;
        float yScale = 1.0f / res_y;

        Minimap Minimap = new Minimap{
            width = xScale * (res_x / (4.0f * aspect_ratio)),
            height = yScale * (res_y / 5.674f),
            left_x = xScale * (res_x * (safeZone_x * ((Absolute(safeZone - 1.0f)) * 10.0f))),
            bottom_y = 1.0f - yScale * (res_y * (safeZone_y * Absolute(safeZone - 1.0f) * 10.0f))
        };
        Minimap.right_x = Minimap.left_x + Minimap.width;
        Minimap.top_y = Minimap.bottom_y - Minimap.height;
        Minimap.x = Minimap.left_x;
        Minimap.y = Minimap.top_y;
        Minimap.xunit = xScale;
        Minimap.yunit = yScale;
        return Minimap;
    }

    private static float Absolute(float value){
        return value < 0 ? -value : value;
    }

    public class Minimap{
        public float width;
        public float height;
        public float left_x;
        public float bottom_y;
        public float right_x;
        public float top_y;
        public float x;
        public float y;
        public float xunit;
        public float yunit;
    }
}