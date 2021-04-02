/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System.Dynamic;

namespace OneScript.Values
{
    public abstract class BslValue : DynamicObject
    {
        protected virtual string ConvertToString() => ToString();
        
        public static explicit operator string(BslValue value)
        {
            return value.ConvertToString();
        }
    }
}