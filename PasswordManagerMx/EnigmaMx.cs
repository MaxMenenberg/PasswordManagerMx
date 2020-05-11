using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManagerMx
{
    /*
     * This class is a implementation of an engima machine that encrypts 94 ascii characters
     * Author: Max Menenberg
     */
    class EnigmaMx
    {

        private int countS = 0;
        private int countM = 0;
        private int countF = 0;

        private int[] rS =
            {79,57,14,75,93,24,8,41,59,11,3,32,77,28,37,7,35,31,63,64,25,70,46,
            23,33,19,6,73,67,36,38,52,17,13,92,65,2,55,44,69,1,49,89,50,48,
            61,85,83,58,82,80,68,86,60,45,76,12,27,78,34,30,53,4,0,29,91,72,
            43,90,47,22,21,16,81,62,39,66,15,40,71,26,56,88,74,54,20,18,9,87,
            42,10,5,84,51};

        private int[] rM =
            {61,84,50,3,75,48,71,39,31,70,40,12,16,33,37,63,65,38,9,89,60,22,58,
            13,11,1,52,64,72,32,17,30,26,43,21,45,0,42,76,23,6,24,53,14,59,
            67,91,34,36,51,7,15,2,49,82,18,47,90,92,87,81,25,93,68,27,5,83,41,
            57,62,46,4,86,80,66,85,79,35,20,28,74,77,78,10,44,19,29,54,8,88,56,55,
            73,69};

        private int[] rF =
            {50,76,49,5,16,15,78,43,61,47,85,0,11,21,13,63,19,32,89,17,48,44,39,86,26,
            59,34,75,24,30,84,62,42,8,36,4,65,64,79,66,14,41,1,28,60,35,51,91,46,3,
            12,71,53,93,74,25,9,92,72,80,88,40,22,58,6,2,10,73,67,29,83,27,31,55,70,
            23,7,54,82,90,57,68,37,87,45,20,52,33,69,56,18,38,77,81};

        //Reflect elements must be mapped in pairs.
        // So if 24 is in the 0th spot 0 must be in the 
        //24th spot. For all elements
        private int[] reflect =
            {35,59,33,37,34,43,46,85,18,31,53,45,24,83,82,55,40,17,8,50,71,21,54,42,
            12,61,32,30,28,89,27,9,26,2,4,0,36,3,84,39,16,62,23,5,76,11,6,63,48,
            64,19,92,52,10,22,15,56,57,69,1,66,25,41,47,49,80,60,75,68,58,70,20,72,
            73,74,67,44,88,78,79,65,81,14,13,38,7,86,87,77,29,90,91,51,93};

        //Starting positions for the Rotors;
        //pos1 for rS, pos2 for rM, pos3, for rF
        public EnigmaMx(int pos1, int pos2, int pos3)
        {
            for (int n = 0; n < pos1; n++)
            {
                shift(rS);
            }
            for (int n = 0; n < pos2; n++)
            {
                shift(rM);
            }
            for (int n = 0; n < pos3; n++)
            {
                shift(rF);
            }
        }

        public String Encrypt(String message) {
            String retval = null;
            char[] word = message.ToCharArray();
            char[] Crypt = new char[word.Length];
            int L = word.Length;
            for (int n = 0; n < L; n++) {
                int rSin = word[n] - 32;

                int rMin = map(rS, rSin);
                int rFin = map(rM, rMin);
                int Refin = map(rF, rFin);

                int rFin2 = map(reflect, Refin);

                int rMin2 = inverse(rF, rFin2);
                int rSin2 = inverse(rM, rMin2);
                int output = inverse(rS, rSin2);
                Crypt[n] = (char)(output + 32);
                shift(rF);
                countF++;
                if (countF > 92)
                {
                    countM++;
                    shift(rM);
                    countF = 0;
                }
                if (countM > 92)
                {
                    countS++;
                    shift(rS);
                    countM = 0;
                }
                if (countS > 92)
                {
                    countS = 0;
                }
            }
            retval = new String(Crypt);
            return retval;
        }

        //Maps one letter to another
        private int map(int[] R, int input){
            for (int n = 0; n < R.Length; n++)
            {
                if (input == R[n])
                {
                    return n;
                }
            }
            return -1;
        }

        //Inverse of map method
        private int inverse(int[] R, int input)
        {
            for (int n = 0; n < R.Length; n++)
            {
                if (input == n)
                {
                    return R[n];
                }
            }
            return -1;
        }

        //Shifts elements in an array to the right by 1
        private void shift(int[] x)
        {
            int L = x.Length;
            int last = x[L - 1];
            for (int n = L - 2; n >= 0; n--)
            {
                x[n + 1] = x[n];
            }
            x[0] = last;
        }
    }
}
