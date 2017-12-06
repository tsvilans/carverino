/**
 * Carverino, a plug-in for Rhinoceros 3d and Grasshopper based
 * on the .NET wrapper for Carve's CSG and mesh boolean operations
 * by Mehran Maghoumi (https://www.maghoumi.com)
 * adapted by Tom Svilans
 *
 * Copyright (C) 2017 Tom Svilans (http://tomsvilans.com)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
 
using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace CarveGH
{
    public class CarveGHInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "CarveGH";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "A GH wrapper for the Carve CSG library.";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("74c53d21-dc30-4098-bee4-6ff533aa8884");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Tom Svilans";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "tsvi@kadk.dk";
            }
        }
    }
}
