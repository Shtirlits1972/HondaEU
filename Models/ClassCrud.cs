using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using HondaEU.Models.Dto;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace HondaEU.Models
{
    public class ClassCrud
    {
        public static List<CarTypeInfo> GetListCarTypeInfo(string vin)
        {
            List<CarTypeInfo> list = null;

            try
            {
                string nfrmpf = vin.Substring(0, 11);
                string nfrmseqepc = vin.Substring(9, 8);

                #region strCommand
                int y = 0;

                string strCommand = " SELECT DISTINCT " +
                        "  pmotyt.hmodtyp vehicle_id, " +
                        "  pmotyt.cmodnamepc model_name, " +
                        "  pmotyt.xcardrs, " +
                        "  pmotyt.dmodyr, " +
                        "  pmotyt.xgradefulnam, " +
                        "  pmotyt.ctrsmtyp, " +
                        "  pmotyt.cmftrepc, " +
                        "  pmotyt.carea, " +
                        "  pmotyt.nengnpf " +
                        "  FROM pmodlt " +
                        "     JOIN pmotyt " +
                        "         ON(pmodlt.cmodnamepc = pmotyt.cmodnamepc " +
                        "         AND pmodlt.dmodyr = pmotyt.dmodyr " +
                        "         AND pmodlt.xcardrs = pmotyt.xcardrs), " +
                        " 	 pmdldt " +
                        " WHERE(pmotyt.nfrmpf = @nfrmpf ) " +
                        "     AND(pmotyt.nfrmseqepcstrt <= @nfrmseqepc ) " +
                        "     AND(pmotyt.nfrmseqepcend >= @nfrmseqepc ) " +
                        "         AND((NOT EXISTS(SELECT 1 " +
                        "                             FROM pmdldt " +
                        "                            WHERE pmdldt.cmodnamepc = pmodlt.cmodnamepc " +
                        "                              AND pmdldt.dmodyr = pmodlt.dmodyr " +
                        "                              AND pmdldt.xcardrs = pmodlt.xcardrs " +
                        "                              AND pmdldt.cmftrepc = pmodlt.cmftrepc) " +
                        "               AND pmodlt.dmodlnch < now()) " +
                        "          OR( " +
                        "                   EXISTS(SELECT 1 " +
                        "                         FROM pmdldt " +
                        "                        WHERE pmdldt.cmodnamepc = pmodlt.cmodnamepc " +
                        "                          AND pmdldt.dmodyr = pmodlt.dmodyr " +
                        "                          AND pmdldt.xcardrs = pmodlt.xcardrs " +
                        "                          AND pmdldt.cmftrepc = pmodlt.cmftrepc " +
                        "                          AND pmdldt.dmodlnch < now())))";

                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<CarTypeInfo>(strCommand, new { nfrmpf, nfrmseqepc }).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }

            return list;
        }
        private static string strConn = Ut.GetMySQLConnect();
        public static readonly string getLang = " SELECT DISTINCT lang.clangjap FROM lang  WHERE lang.code = @code ";
        public static List<ModelCar> GetModelCars()
        {
            List<ModelCar> list = null;
            #region MyRegion
            string strCommand = " SELECT DISTINCT cmodnamepc model_id, cmodnamepc name, " +
                                " REPLACE(cmodnamepc, ' ', '-')  seo_url FROM pmodlt; ";
            #endregion
            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<ModelCar>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<lang> GetLang()
        {
            List<lang> list = new List<lang>();
            string strCommand = "   SELECT code, name, is_default FROM lang; ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<lang>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<string> GetWmi()
        {
            List<string> list = new List<string>();
            string strCommand = " SELECT value FROM wmi; ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<string>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<node> GetNodes(string[] codesArr = null, string[] node_idsArr = null)
        {
            List<node> list = new List<node>();
            string codes = null;
            string node_ids = null;

            if (codesArr != null && codesArr.Length > 0)
            {
                codes = string.Empty;

                for (int i = 0; i < codesArr.Length; i++)
                {
                    if (i == 0)
                    {
                        codes += codesArr[i];
                    }
                    else
                    {
                        codes += "," + codesArr[i];
                    }

                }
            }


            if (node_idsArr != null && node_idsArr.Length > 0)
            {
                node_ids = string.Empty;

                for (int i = 0; i < node_idsArr.Length; i++)
                {
                    if (i == 0)
                    {
                        node_ids += node_idsArr[i];
                    }
                    else
                    {
                        node_ids += "," + node_idsArr[i];
                    }
                }
            }

            //string strCommand = " SELECT nt.code, nt.group name, nt.node_ids FROM " +
            //                    " nodes_tb nt ";

            string strCommand = " SELECT nt.code, nt.xplblk name, nt.id node_ids FROM " +
                                " pbldst nt ";

            if (!String.IsNullOrEmpty(codes) || !String.IsNullOrEmpty(node_ids))
            {
                strCommand += " WHERE";
            }

            if (!String.IsNullOrEmpty(codes))
            {
                strCommand += $"  nt.code IN  ({codes}) ";
            }

            if (!String.IsNullOrEmpty(codes) && !String.IsNullOrEmpty(node_ids))
            {
                strCommand += " OR ";
            }

            if (!String.IsNullOrEmpty(node_ids))
            {
                strCommand += $"  nt.id IN  ({node_ids}) ";
            }

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<node>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<Sgroups> GetSgroups(string vehicle_id, string group_id, string code_lang = "EN")
        {
            List<Sgroups> list = null;
            //   pbldst.npl, '_', pblokt.nplblk
            string npl = group_id.Substring(0, group_id.IndexOf("_"));
            string nplblk = group_id.Substring(group_id.IndexOf("_") + 1, group_id.Length - (group_id.IndexOf("_") + 1));

            try
            {
                #region strCommand
                string strCommand = "  SELECT DISTINCT " +
                                    //"   CONCAT(pbldst.npl, '_', pblokt.nplblk) node_id , " +
                                    "   pbldst.id node_id , " +

                                    "   pbldst.xplblk  name, " +
                                    "   pblokt.nplblk image_id, " +
                                    " '.png' image_ext " +
                                    "      FROM pblokt " +
                                    "      INNER JOIN pbldst " +
                                    "              ON pblokt.npl = pbldst.npl " +
                                    "             AND pblokt.nplblk = pbldst.nplblk " +
                                    $"             and pbldst.clangjap IN ({getLang}) " +
                                    $"  and pbldst.npl = @npl " +
                                    $"  WHERE pblokt.nplblk = @nplblk and " +
                                    "   (pblokt.nplblk in (SELECT DISTINCT pblmtt.nplblk " +
                                    "   FROM pblmtt   WHERE " +
                                    $" (pblmtt.npl =  @npl ) AND " +
                                    $"   (pblmtt.hmodtyp = @hmodtyp   )) ) " +
                                    "  ORDER BY pbldst.xplblk ; ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<Sgroups>(strCommand, new { hmodtyp = vehicle_id, nplblk, npl, code = code_lang }).ToList();
                }
            }
            catch (Exception ex)
            {
                string Errror = ex.Message;
                int y = 0;
            }

            return list;
        }
        public static List<header> GetHeaders()
        {
            List<header> list = new List<header>();

            //header header1 = new header { code = "vehicle_id", title = "Код типа автомобиля" };
            //list.Add(header1);
            header header2 = new header { code = "model_name", title = "Модель автомобиля" };
            list.Add(header2);
            header header3 = new header { code = "xcardrs", title = "Кол-во дверей" };
            list.Add(header3);
            header header4 = new header { code = "dmodyr", title = "Год выпуска" };
            list.Add(header4);
            header header5 = new header { code = "xgradefulnam", title = "Класс" };
            list.Add(header5);
            header header6 = new header { code = "ctrsmtyp", title = "Тип трансмиссии" };
            list.Add(header6);
            header header7 = new header { code = "cmftrepc", title = "Страна производитель" };
            list.Add(header7);
            header header8 = new header { code = "carea", title = "Страна рынок" };
            list.Add(header8);
            header header9 = new header { code = "nengnpf", title = "Код двигателя" };
            list.Add(header9);

            return list;
        }
        public static List<PartsGroup> GetPartsGroup(string vehicle_id, string code_lang = "EN")
        {
            List<PartsGroup> list = new List<PartsGroup>();
            string strCommand = " SELECT DISTINCT " +
                                " CONCAT(pgrout.nplgrp, '_', pblokt.npl) group_id, " +
                                    //" CONCAT(pgrout.clangjap, '_', pgrout.nplgrp, '_', pblokt.npl) group_id, " +
                                " pgrout.xplgrp name FROM pgrout " +
                                " LEFT JOIN pblokt ON pgrout.nplgrp = pblokt.nplgrp " +
                                " LEFT JOIN pblmtt ON pblokt.npl = pblmtt.npl " +
                                " AND pblokt.nplblk = pblmtt.nplblk " +
                               $" WHERE pgrout.clangjap IN({getLang}) " +
                                " AND EXISTS(SELECT 1 " +
                                " FROM pblokt, pblmtt " +
                                " WHERE pblokt.nplgrp = pgrout.nplgrp " +
                                " AND pblokt.npl = pblmtt.npl " +
                                " AND pblokt.nplblk = pblmtt.nplblk " +
                                " AND pblmtt.hmodtyp = @hmodtyp) " +
                                " AND pblmtt.hmodtyp = @hmodtyp ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<PartsGroup>(strCommand, new { hmodtyp = vehicle_id, code = code_lang }).ToList();
                }

                for (int i = 0; i < list.Count; i++)
                {
                    list[i].childs = GetPartsGroupChild(vehicle_id, list[i].group_id, code_lang);
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<PartsGroup> GetPartsGroupChild(string vehicle_id, string group_id, string code_lang = "EN")
        {
            List<PartsGroup> list = new List<PartsGroup>();

            string[] strArrGroup = group_id.Split("_");

            string nplgrp = strArrGroup[0];
            string npl = strArrGroup[1];

            try
            {
                #region strCommand
                string strCommand = "  SELECT DISTINCT " +
                                    "   CONCAT(pbldst.npl, '_', pblokt.nplblk)  group_id, " +
                                    "   pbldst.xplblk  name, " +
                                    "   pblokt.nplblk image_id, " +
                                    " '.png' image_ext " +
                                    "      FROM pblokt " +
                                    "           JOIN pbldst " +
                                    "              ON pblokt.npl = pbldst.npl " +
                                    "             AND pblokt.nplblk = pbldst.nplblk " +
                                    $"             and pbldst.clangjap IN ({getLang}) " +
                                    $"  and pbldst.npl = @npl " +
                                    $"  WHERE pblokt.nplgrp = @nplgrp and " +
                                    "   (pblokt.nplblk in (SELECT DISTINCT pblmtt.nplblk " +
                                    "   FROM pblmtt   WHERE " +
                                    $" (pblmtt.npl =  @npl ) AND " +
                                    $"   (pblmtt.hmodtyp = @hmodtyp   )) ) " +
                                    "  ORDER BY pbldst.xplblk ; ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<PartsGroup>(strCommand, new { hmodtyp = vehicle_id, nplgrp, npl, code = code_lang }).ToList();
                }
            }
            catch (Exception ex)
            {
                string Errror = ex.Message;
                int y = 0;
            }
            return list;
        }
        public static List<Filters> GetFilters(string model_id, string xcardrsP, string dmodyrP, string xgradefulnamP, string ctrsmtypP, string cmftrepcP,
            string careaP, string nengnpfP)
        {

            List<Filters> filters = new List<Filters>();
            List<CarTypeInfo> listCarType = new List<CarTypeInfo>();

            string strWhereAdd = string.Empty;

            string strCommand = " SELECT p.hmodtyp vehicle_id, cmodnamepc model_name,  " +
                                " xcardrs, dmodyr, xgradefulnam, ctrsmtyp, cmftrepc, carea, nengnpf " +
                                " FROM pmotyt p WHERE p.cmodnamepc = @model_id  ";


            if(!String.IsNullOrEmpty(xcardrsP))
            {
                string maneParam = xcardrsP.Substring(0, xcardrsP.IndexOf("_"));
                string tmpVal = xcardrsP.Substring(xcardrsP.IndexOf("_") + 1);
                string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                strCommand += tmpParam;
            }
            if (!String.IsNullOrEmpty(dmodyrP))
            {
                string maneParam = dmodyrP.Substring(0, dmodyrP.IndexOf("_"));
                string tmpVal = dmodyrP.Substring(dmodyrP.IndexOf("_") + 1);
                string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                strCommand += tmpParam;
            }
            if (!String.IsNullOrEmpty(xgradefulnamP))
            {
                string maneParam = xgradefulnamP.Substring(0, xgradefulnamP.IndexOf("_"));
                string tmpVal = xgradefulnamP.Substring(xgradefulnamP.IndexOf("_") + 1);
                string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                strCommand += tmpParam;
            }
            if (!String.IsNullOrEmpty(ctrsmtypP))
            {
                string maneParam = ctrsmtypP.Substring(0, ctrsmtypP.IndexOf("_"));
                string tmpVal = ctrsmtypP.Substring(ctrsmtypP.IndexOf("_") + 1);
                string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                strCommand += tmpParam;
            }
            if (!String.IsNullOrEmpty(cmftrepcP))
            {
                string maneParam = cmftrepcP.Substring(0, cmftrepcP.IndexOf("_"));
                string tmpVal = cmftrepcP.Substring(cmftrepcP.IndexOf("_") + 1);
                string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                strCommand += tmpParam;
            }
            if (!String.IsNullOrEmpty(careaP))
            {
                string maneParam = careaP.Substring(0, careaP.IndexOf("_"));
                string tmpVal = careaP.Substring(careaP.IndexOf("_") + 1);
                string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                strCommand += tmpParam;
            }
            if (!String.IsNullOrEmpty(nengnpfP))
            {
                string maneParam = nengnpfP.Substring(0, nengnpfP.IndexOf("_"));
                string tmpVal = nengnpfP.Substring(nengnpfP.IndexOf("_") + 1);
                string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                strCommand += tmpParam;
            }

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    listCarType = db.Query<CarTypeInfo>(strCommand, new { model_id }).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }

            #region xcardrs
            Filters xcardrsF = new Filters { filter_id = "xcardrs", code = "xcardrs", name = "К-во дверей" };
            List<values> xcardrs_Val = new List<values>();

            string[] xcardrsArr = listCarType.Select(x => x.xcardrs).Distinct().ToArray();

            for (int i = 0; i < xcardrsArr.Length; i++)
            {
                values xcardrs_tmp = new values { filter_item_id = "xcardrs_" + xcardrsArr[i], name = xcardrsArr[i] };
                xcardrs_Val.Add(xcardrs_tmp);
            }

            xcardrsF.values = xcardrs_Val;
            filters.Add(xcardrsF);
            #endregion

            #region dmodyr
            Filters dmodyrF = new Filters { filter_id = "dmodyr", code = "dmodyr", name = "Год выпуска" };
            List<values> dmodyr_Val = new List<values>();

            string[] dmodyrArr = listCarType.Select(x => x.dmodyr).Distinct().ToArray();

            for (int i = 0; i < dmodyrArr.Length; i++)
            {
                values dmodyr_tmp = new values { filter_item_id = "dmodyr_" + dmodyrArr[i], name = dmodyrArr[i] };
                dmodyr_Val.Add(dmodyr_tmp);
            }

            dmodyrF.values = dmodyr_Val;
            filters.Add(dmodyrF);
            #endregion

            #region xgradefulnam
            Filters xgradefulnamF = new Filters { filter_id = "xgradefulnam", code = "xgradefulnam", name = "Класс" };
            List<values> xgradefulnam_Val = new List<values>();

            string[] xgradefulnamArr = listCarType.Select(x => x.xgradefulnam).Distinct().ToArray();

            for (int i = 0; i < xgradefulnamArr.Length; i++)
            {
                values xgradefulnam_tmp = new values { filter_item_id = "xgradefulnam_" + xgradefulnamArr[i], name = xgradefulnamArr[i] };
                xgradefulnam_Val.Add(xgradefulnam_tmp);
            }

            xgradefulnamF.values = xgradefulnam_Val;
            filters.Add(xgradefulnamF);
            #endregion

            #region ctrsmtyp
            Filters ctrsmtypF = new Filters { filter_id = "ctrsmtyp", code = "ctrsmtyp", name = "Тип трансмиссии" };
            List<values> ctrsmtyp_Val = new List<values>();

            string[] ctrsmtypArr = listCarType.Select(x => x.ctrsmtyp).Distinct().ToArray();

            for (int i = 0; i < ctrsmtypArr.Length; i++)
            {
                values ctrsmtyp_tmp = new values { filter_item_id = "ctrsmtyp_" + ctrsmtypArr[i], name = ctrsmtypArr[i] };
                ctrsmtyp_Val.Add(ctrsmtyp_tmp);
            }

            ctrsmtypF.values = ctrsmtyp_Val;
            filters.Add(ctrsmtypF);
            #endregion

            #region cmftrepc
            Filters cmftrepcF = new Filters { filter_id = "cmftrepc", code = "cmftrepc", name = "Страна пр-ва" };
            List<values> cmftrepc_Val = new List<values>();

            string[] cmftrepcArr = listCarType.Select(x => x.cmftrepc).Distinct().ToArray();

            for (int i = 0; i < cmftrepcArr.Length; i++)
            {
                values cmftrepc_tmp = new values { filter_item_id = "cmftrepc_" + cmftrepcArr[i], name = cmftrepcArr[i] };
                cmftrepc_Val.Add(cmftrepc_tmp);
            }

            cmftrepcF.values = cmftrepc_Val;
            filters.Add(cmftrepcF);
            #endregion

            #region carea
            Filters careaF = new Filters { filter_id = "carea", code = "carea", name = "Страна рынок" };
            List<values> carea_Val = new List<values>();

            string[] careaArr = listCarType.Select(x => x.carea).Distinct().ToArray();

            for (int i = 0; i < careaArr.Length; i++)
            {
                values carea_tmp = new values { filter_item_id = "carea_" + careaArr[i], name = careaArr[i] };
                carea_Val.Add(carea_tmp);
            }

            careaF.values = carea_Val;
            filters.Add(careaF);
            #endregion

            #region nengnpf
            Filters nengnpfF = new Filters { filter_id = "nengnpf", code = "nengnpf", name = "Код двигателя" };
            List<values> nengnpf_Val = new List<values>();

            string[] nengnpfArr = listCarType.Select(x => x.nengnpf).Distinct().ToArray();

            for (int i = 0; i < nengnpfArr.Length; i++)
            {
                values nengnpf_tmp = new values { filter_item_id = "nengnpf_" + nengnpfArr[i], name = nengnpfArr[i] };
                nengnpf_Val.Add(nengnpf_tmp);
            }

            nengnpfF.values = nengnpf_Val;
            filters.Add(nengnpfF);
            #endregion

            return filters;
        }
        public static List<CarTypeInfo> GetListCarTypeInfoFilterCars(string model_id, string xcardrsP, string dmodyrP, string xgradefulnamP, string ctrsmtypP,
            string cmftrepcP, string careaP, string nengnpfP)
        {
            List<CarTypeInfo> list = null;

            try
            {
                #region strCommand
                string strCommand = "   SELECT DISTINCT " +
                                    "   p.hmodtyp vehicle_id, " +
                                    " 	p.cmodnamepc model_name, " +
                                    " 	p.xcardrs, " +
                                    " 	p.dmodyr, " +
                                    " 	p.xgradefulnam, " +
                                    " 	p.ctrsmtyp, " +
                                    " 	p.cmftrepc, " +
                                    "   p.carea, " +
                                    "   p.nengnpf " +
                                    "   FROM pmotyt p " +
                                    "   WHERE p.cmodnamepc = @model_id ";

                if (!String.IsNullOrEmpty(xcardrsP))
                {
                    string maneParam = xcardrsP.Substring(0, xcardrsP.IndexOf("_"));
                    string tmpVal = xcardrsP.Substring(xcardrsP.IndexOf("_") + 1);
                    string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                    strCommand += tmpParam;
                }
                if (!String.IsNullOrEmpty(dmodyrP))
                {
                    string maneParam = dmodyrP.Substring(0, dmodyrP.IndexOf("_"));
                    string tmpVal = dmodyrP.Substring(dmodyrP.IndexOf("_") + 1);
                    string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                    strCommand += tmpParam;
                }
                if (!String.IsNullOrEmpty(xgradefulnamP))
                {
                    string maneParam = xgradefulnamP.Substring(0, xgradefulnamP.IndexOf("_"));
                    string tmpVal = xgradefulnamP.Substring(xgradefulnamP.IndexOf("_") + 1);
                    string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                    strCommand += tmpParam;
                }
                if (!String.IsNullOrEmpty(ctrsmtypP))
                {
                    string maneParam = ctrsmtypP.Substring(0, ctrsmtypP.IndexOf("_"));
                    string tmpVal = ctrsmtypP.Substring(ctrsmtypP.IndexOf("_") + 1);
                    string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                    strCommand += tmpParam;
                }
                if (!String.IsNullOrEmpty(cmftrepcP))
                {
                    string maneParam = cmftrepcP.Substring(0, cmftrepcP.IndexOf("_"));
                    string tmpVal = cmftrepcP.Substring(cmftrepcP.IndexOf("_") + 1);
                    string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                    strCommand += tmpParam;
                }
                if (!String.IsNullOrEmpty(careaP))
                {
                    string maneParam = careaP.Substring(0, careaP.IndexOf("_"));
                    string tmpVal = careaP.Substring(careaP.IndexOf("_") + 1);
                    string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                    strCommand += tmpParam;
                }
                if (!String.IsNullOrEmpty(nengnpfP))
                {
                    string maneParam = nengnpfP.Substring(0, nengnpfP.IndexOf("_"));
                    string tmpVal = nengnpfP.Substring(nengnpfP.IndexOf("_") + 1);
                    string tmpParam = $" AND p.{maneParam} = '{tmpVal}' ";

                    strCommand += tmpParam;
                }

                //  nengnpfP
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<CarTypeInfo>(strCommand, new { model_id }).ToList();
                }
            }
            catch (Exception ex)
            {
                string Errror = ex.Message;
                int y = 0;
            }

            return list;
        }
        public static VehiclePropArr GetVehiclePropArr(string vehicle_id)
        {
            VehiclePropArr model = null;

            try
            {
                #region strCommand
                string strCommand = "  SELECT DISTINCT " +
                                    "  p.hmodtyp vehicle_id, " +
                                    "  p.cmodnamepc model_name, " +
                                    "  p.xcardrs, " +
                                    "  p.dmodyr, " +
                                    "  p.xgradefulnam, " +
                                    "  p.ctrsmtyp, " +
                                    "  p.cmftrepc, " +
                                    "  p.carea, " +
                                    "  p.nengnpf " + 
                                    "  FROM pmotyt p " +
                                    "  WHERE p.hmodtyp = @vehicle_id LIMIT 1; ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    CarTypeInfo carType = db.Query<CarTypeInfo>(strCommand, new { vehicle_id }).FirstOrDefault();

                    List<attributes> list = GetAttributes();

                    list[0].value = carType.model_name;
                    list[1].value = carType.xcardrs;
                    list[2].value = carType.dmodyr;
                    list[3].value = carType.xgradefulnam;
                    list[4].value = carType.ctrsmtyp;
                    list[5].value = carType.cmftrepc;
                    list[6].value = carType.carea;
                    list[7].value = carType.nengnpf;

                    model = new VehiclePropArr {  model_name = carType.model_name };
                    model.attributes = list;
                }
            }
            catch (Exception ex)
            {
                string Errror = ex.Message;
                int y = 0;
            }

            return model;
        }
        public static List<attributes> GetAttributes()
        {
            List<attributes> list = new List<attributes>();

            attributes cmodnamepc = new attributes { code = "model_name", name = "Модель автомобиля", value = "" };
            list.Add(cmodnamepc);

            attributes xcardrs = new attributes { code = "xcardrs", name = "Кол-во дверей", value = "" };
            list.Add(xcardrs);

            attributes dmodyr = new attributes { code = "dmodyr", name = "Год выпуска", value = "" };
            list.Add(dmodyr);

            attributes xgradefulnam = new attributes { code = "xgradefulnam", name = "Класс", value = "" };
            list.Add(xgradefulnam);

            attributes ctrsmtyp = new attributes { code = "ctrsmtyp", name = "Тип трансмиссии", value = "" };
            list.Add(ctrsmtyp);

            attributes cmftrepc = new attributes { code = "cmftrepc", name = "Страна производитель", value = "" };
            list.Add(cmftrepc);

            attributes carea = new attributes { code = "carea", name = "Страна, рынок", value = "" };
            list.Add(carea);

            attributes nengnpf = new attributes { code = "nengnpf", name = "Класс двигателя", value = "" };
            list.Add(nengnpf);
            //  
            return list;
        }
        public static DetailsInNode GetDetailsInNode(string vehicle_id, string node_id, string lang = "EN")
        {
            DetailsInNode detailsInNode = new DetailsInNode { node_id = node_id };

            string imgPath = Ut.GetImagePath();

            string strCommand = " SELECT DISTINCT pbldst.xplblk " +
                                " FROM " +
                                " pbldst " +
                                " WHERE   pbldst.id = @node_id ";

            string strCommDeatil = " SELECT DISTINCT " +
                                    " ppartt.npartgenu number, " +
                                    " ppartt.xpartext name, " +

                                    " pblpat.npl,  " +
                                    " pblpat.nplblk,  " +
                                    " pblpat.nplpartref " +

                                    " FROM pblpat " +
                                    " join ppartt on(ppartt.clangjap IN (SELECT DISTINCT lang.clangjap FROM lang WHERE lang.code = @code ) " +
                                    " and ppartt.npartgenu = pblpat.npartgenu) " +
                                    " join ppasat on(ppartt.npartgenu = ppasat.npartgenu) " +
                                    " left outer join pbprmt on(pbprmt.hpartplblk = pblpat.hpartplblk " +
                                    " and pbprmt.clangjap  IN(SELECT DISTINCT lang.clangjap FROM lang  WHERE lang.code = @code )) " +
                                    " WHERE pblpat.npl IN(SELECT DISTINCT npl FROM pbldst WHERE pbldst.id = @node_id) AND " +
                                    " pblpat.nplblk IN(SELECT DISTINCT nplblk FROM pbldst WHERE pbldst.id = @node_id) AND " +
                                    " ((EXISTS(SELECT pb.hmodtyp " +
                                    " FROM pbpmtt AS pb " +
                                    " WHERE pb.hpartplblk = pblpat.hpartplblk AND " +
                                    " pb.hmodtyp IN(@vehicle_id))) OR " +
                                    " ((NOT EXISTS(SELECT pb.hmodtyp " +
                                    " FROM pbpmtt AS pb " +
                                    " WHERE pb.hpartplblk = pblpat.hpartplblk)) " +
                                    " AND " +
                                    " (EXISTS(SELECT pb.hmodtyp " +
                                    " FROM pblmtt AS pb " +
                                    " WHERE pb.npl = pblpat.npl AND " +
                                    " pb.nplblk = pblpat.nplblk AND " +
                                    " pb.hmodtyp IN( @vehicle_id ))))); ";


            string strCommImages = " SELECT DISTINCT CONCAT(pbldst.npl, '_', pbldst.nplblk, '-png') image_id, '.png' ext, " +
                        $" CONCAT( '{imgPath}', pbldst.npl, '/' , 'IMGE/', pbldst.nplblk, '.png') path " +   //     //    pbldst.npl, 'IMGE+', 
                        " FROM pbldst WHERE " +

                        " pbldst.nplblk IN ( SELECT DISTINCT nplblk FROM pbldst WHERE pbldst.id = @node_id )  " +
                        " AND pbldst.npl IN ( SELECT DISTINCT npl FROM pbldst WHERE pbldst.id = @node_id ) " +

                        $" AND pbldst.clangjap IN ({getLang}) ; ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    detailsInNode.name = db.Query<string>(strCommand, new { node_id, code = lang }).FirstOrDefault();
                    detailsInNode.parts = db.Query<Detail>(strCommDeatil, new { vehicle_id, node_id, code = lang }).ToList();
                    detailsInNode.images = db.Query<images>(strCommImages, new { node_id, code = lang }).ToList();
                }

                for (int i = 0; i < detailsInNode.parts.Count; i++)
                {
                    detailsInNode.parts[i].hotspots = GetHotspots(detailsInNode.parts[i].nplblk, detailsInNode.parts[i].npl, detailsInNode.parts[i].nplpartref);
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int y = 0;
            }

            return detailsInNode;
        }
        public static List<hotspots> GetHotspots(string illustrationnumber, string npl, string partreferencenumber)
        {
            List<hotspots> list = new List<hotspots>();
            try
            {
                #region strCommand
                string strCommand = "   SELECT " +
                                    "   h.partreferencenumber hotspot_id, " +
                                    "   CONCAT(h.npl, '_', h.illustrationnumber, '-', 'png' ) image_id, " +
                                    "   h.max_x x2, " +
                                    "   h.max_y y2, " +
                                    "   h.min_x x1, " +
                                    "   h.min_y y1  " +
                                    "   FROM hotspots h " +
                                    " WHERE h.illustrationnumber = @illustrationnumber " +
                                    " AND h.npl = @npl  AND h.partreferencenumber = @partreferencenumber;  ";
                #endregion
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<hotspots>(strCommand, new { illustrationnumber, npl, partreferencenumber }).ToList();
                }
            }
            catch (Exception ex)
            {
                string Errror = ex.Message;
                int o = 0;
            }

            return list;
        }
    }
}
