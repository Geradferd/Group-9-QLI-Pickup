/*
 @licstart  The following is the entire license notice for the JavaScript code in this file.

 The MIT License (MIT)

 Copyright (C) 1997-2020 by Dimitri van Heesch

 Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 and associated documentation files (the "Software"), to deal in the Software without restriction,
 including without limitation the rights to use, copy, modify, merge, publish, distribute,
 sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in all copies or
 substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 @licend  The above is the entire license notice for the JavaScript code in this file
*/
var NAVTREE =
[
  [ "QLI_Pickup", "index.html", [
    [ "API Documentation", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html", [
      [ "Base URL", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md4", null ],
      [ "Authentication", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md5", null ],
      [ "Auth Endpoints", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md7", [
        [ "POST /api/auth/register", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md8", null ],
        [ "POST /api/auth/login", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md9", null ],
        [ "POST /api/auth/register-admin", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md10", null ],
        [ "POST /api/auth/register-driver", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md11", null ]
      ] ],
      [ "Rider Endpoints", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md13", [
        [ "GET /api/rider", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md14", null ],
        [ "GET /api/rider/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md15", null ],
        [ "POST /api/rider", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md16", null ],
        [ "PUT /api/rider/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md17", null ],
        [ "DELETE /api/rider/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md18", null ]
      ] ],
      [ "Driver Endpoints", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md20", [
        [ "GET /api/driver", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md21", null ],
        [ "GET /api/driver/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md22", null ],
        [ "POST /api/driver", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md23", null ],
        [ "PUT /api/driver/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md24", null ],
        [ "DELETE /api/driver/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md25", null ]
      ] ],
      [ "Vehicle Endpoints", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md27", [
        [ "GET /api/vehicle", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md28", null ],
        [ "GET /api/vehicle/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md29", null ],
        [ "POST /api/vehicle", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md30", null ],
        [ "PUT /api/vehicle/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md31", null ],
        [ "DELETE /api/vehicle/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md32", null ]
      ] ],
      [ "Transportation Type Endpoints", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md34", [
        [ "GET /api/transportationtypes", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md35", null ],
        [ "GET /api/transportationtypes/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md36", null ],
        [ "POST /api/transportationtypes", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md37", null ],
        [ "PUT /api/transportationtypes/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md38", null ],
        [ "DELETE /api/transportationtypes/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md39", null ]
      ] ],
      [ "Trip Endpoints", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md41", [
        [ "GET /api/trips", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md42", null ],
        [ "GET /api/trips/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md43", null ],
        [ "POST /api/trips", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md44", null ],
        [ "PUT /api/trips/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md45", null ],
        [ "DELETE /api/trips/{id}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md46", null ]
      ] ],
      [ "Trip Status Actions", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md48", [
        [ "POST /api/trips/{id}/approve", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md49", null ],
        [ "POST /api/trips/{id}/deny", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md50", null ],
        [ "POST /api/trips/{id}/assign", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md51", null ],
        [ "POST /api/trips/{id}/start", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md52", null ],
        [ "POST /api/trips/{id}/complete", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md53", null ],
        [ "POST /api/trips/{id}/noshow", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md54", null ],
        [ "POST /api/trips/{id}/cancel", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md55", null ]
      ] ],
      [ "Trip Status History", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md57", [
        [ "GET /api/tripstatushistory/trip/{tripId}", "md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md58", null ]
      ] ]
    ] ],
    [ "Testing Guide", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html", [
      [ "Setup", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md69", null ],
      [ "Step 1: Register a User", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md70", null ],
      [ "Step 2: Promote to Admin", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md71", null ],
      [ "Step 3: Login as Admin", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md72", null ],
      [ "Step 4: Create a Transportation Type", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md73", null ],
      [ "Step 5: Create a Rider", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md74", null ],
      [ "Step 6: Create a Driver", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md75", null ],
      [ "Step 7: Create a Vehicle", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md76", null ],
      [ "Step 8: Create a Trip", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md77", null ],
      [ "Step 9: Approve the Trip", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md78", null ],
      [ "Step 10: Assign Driver and Vehicle", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md79", null ],
      [ "Step 11: Login as Driver", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md80", null ],
      [ "Step 12: Start the Trip", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md81", null ],
      [ "Step 13: Complete the Trip", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md82", null ],
      [ "Step 14: Check the Audit Trail", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md83", null ],
      [ "Testing Invalid Transitions", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md84", null ],
      [ "Verifying in the Database", "md_docs_2api_2_t_e_s_t_i_n_g___g_u_i_d_e.html#autotoc_md85", null ]
    ] ],
    [ "Group-9-QLI-Pickup", "md__r_e_a_d_m_e.html", [
      [ "What is this application?", "md__r_e_a_d_m_e.html#autotoc_md87", null ],
      [ "How to use", "md__r_e_a_d_m_e.html#autotoc_md88", [
        [ "Development Mode", "md__r_e_a_d_m_e.html#autotoc_md89", null ],
        [ "MySQL Startup", "md__r_e_a_d_m_e.html#autotoc_md90", null ],
        [ "Production Mode", "md__r_e_a_d_m_e.html#autotoc_md91", null ]
      ] ],
      [ "Release Notes", "md__r_e_a_d_m_e.html#autotoc_md92", [
        [ "Current Submission - Transportation Management System (April 2, 2026)", "md__r_e_a_d_m_e.html#autotoc_md93", [
          [ "✅ Authentication &amp; Authorization System", "md__r_e_a_d_m_e.html#autotoc_md94", null ],
          [ "✅ Transportation Types API (NEW - Fully Implemented)", "md__r_e_a_d_m_e.html#autotoc_md95", null ],
          [ "✅ Database Models", "md__r_e_a_d_m_e.html#autotoc_md96", null ],
          [ "✅ Infrastructure", "md__r_e_a_d_m_e.html#autotoc_md97", null ],
          [ "🔄 GPS Tracking (Planned)", "md__r_e_a_d_m_e.html#autotoc_md98", null ]
        ] ]
      ] ],
      [ "Recent Changes (April 2, 2026)", "md__r_e_a_d_m_e.html#autotoc_md99", [
        [ "Models Added/Updated", "md__r_e_a_d_m_e.html#autotoc_md100", null ],
        [ "API Endpoints Added", "md__r_e_a_d_m_e.html#autotoc_md101", null ],
        [ "DTOs Added", "md__r_e_a_d_m_e.html#autotoc_md102", null ],
        [ "Configuration Updates", "md__r_e_a_d_m_e.html#autotoc_md103", null ],
        [ "Documentation Added", "md__r_e_a_d_m_e.html#autotoc_md104", null ]
      ] ],
      [ "API Endpoints Summary", "md__r_e_a_d_m_e.html#autotoc_md105", [
        [ "Authentication Endpoints", "md__r_e_a_d_m_e.html#autotoc_md106", null ],
        [ "Transportation Types Endpoints (NEW)", "md__r_e_a_d_m_e.html#autotoc_md107", null ],
        [ "Health Check", "md__r_e_a_d_m_e.html#autotoc_md108", null ]
      ] ],
      [ "How to Test the Transportation Types API", "md__r_e_a_d_m_e.html#autotoc_md109", null ],
      [ "Branches", "md__r_e_a_d_m_e.html#autotoc_md110", null ]
    ] ],
    [ "Packages", "namespaces.html", [
      [ "Package List", "namespaces.html", "namespaces_dup" ]
    ] ],
    [ "Classes", "annotated.html", [
      [ "Class List", "annotated.html", "annotated_dup" ],
      [ "Class Index", "classes.html", null ],
      [ "Class Hierarchy", "hierarchy.html", "hierarchy" ],
      [ "Class Members", "functions.html", [
        [ "All", "functions.html", null ],
        [ "Functions", "functions_func.html", null ],
        [ "Properties", "functions_prop.html", null ]
      ] ]
    ] ]
  ] ]
];

var NAVTREEINDEX =
[
"annotated.html",
"md_docs_2api_2_a_p_i___d_o_c_u_m_e_n_t_a_t_i_o_n.html#autotoc_md4"
];

var SYNCONMSG = 'click to disable panel synchronization';
var SYNCOFFMSG = 'click to enable panel synchronization';
var LISTOFALLMEMBERS = 'List of all members';