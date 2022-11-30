using Godot;

namespace GodotMidasDepth.Math; 

public partial class MachineEpsilon {
    public static float CalculateMachineEpsilonForFloat()
    {

        GD.Print( "Float:" );

        float machineEpsilon = 1.0f;
        float x = 0.0f;
        int loopCount = 0;

        do
        {
            machineEpsilon /= 2.0f;
            x = 1.0f + machineEpsilon;
            loopCount++;
            GD.Print( "\t" + loopCount.ToString( "00" ) + ") " + machineEpsilon.ToString() );
        }
        while ( x > 1.0 );

        GD.Print( "\n\tMantissa Bit Count: " + loopCount );
        GD.Print( "\tMachine epsilon for float: " + 2 * machineEpsilon );

        return machineEpsilon;
    }
}