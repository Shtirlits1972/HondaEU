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

            string strCommand = " SELECT nt.code, nt.group name, nt.node_ids FROM " +
                                " nodes_tb nt ";

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
                strCommand += $"  nt.node_ids IN  ({node_ids}) ";
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

            string[] strArr = group_id.Split("_");

            string nplgrp = strArr[1];
            string npl = strArr[2];

            try
            {
                #region strCommand
                string strCommand = "  SELECT DISTINCT " +
                                    "   CONCAT(pbldst.npl, '_', pblokt.nplblk) node_id , " +
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
                    list = db.Query<Sgroups>(strCommand, new { hmodtyp = vehicle_id, nplgrp, npl, code = code_lang }).ToList();
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

            header header1 = new header { code = "vehicle_id", title = "Код типа автомобиля" };
            list.Add(header1);
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

            return list;
        }
        public static List<PartsGroup> GetPartsGroup(string vehicle_id, string code_lang = "EN")
        {
            List<PartsGroup> list = null;
            string strCommand = " SELECT DISTINCT " +
                                " CONCAT(pgrout.clangjap, '_', pgrout.nplgrp, '_', pblokt.npl) Id, " +
                                " pgrout.xplgrp name FROM pgrout " +
                                " LEFT JOIN pblokt ON pgrout.nplgrp = pblokt.nplgrp " +
                                " LEFT JOIN pblmtt ON pblokt.npl = pblmtt.npl " +
                                " AND pblokt.nplblk = pblmtt.nplblk " +
                                " WHERE pgrout.clangjap IN(SELECT DISTINCT " +
                                " lang.clangjap " +
                                " FROM lang " +
                                " WHERE lang.code = @code ) " +
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
                    List<PartsGroup> listSgroups = GetPartsGroupChild(vehicle_id, list[i].group_id, code_lang);
                    list[i].childs = listSgroups;
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
            List<PartsGroup> list = null;

            string[] strArr = vehicle_id.Split("_");

            string catalog = strArr[0];
            string catalog_code = strArr[1];

            string[] strArrGroup = group_id.Split("_");

            string nplgrp = strArrGroup[1];
            string npl = strArrGroup[2];

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

        public static List<Filters> GetFilters(string model_id, string[] param)
        {
            List<Filters> filters = new List<Filters>();
            List<CarTypeInfo> listCarType = new List<CarTypeInfo>();

            string strWhereAdd = string.Empty;

            string strCommand = " SELECT p.hmodtyp vehicle_id, cmodnamepc model_name,  " +
                                " xcardrs, dmodyr, xgradefulnam, ctrsmtyp, cmftrepc, carea " +
                                " FROM pmotyt p WHERE p.cmodnamepc = @model_id  ";

            for(int i=0; i < param.Length; i++)
            {
                string maneParam = param[i].Substring(0, param[i].IndexOf("_"));
                string tmpVal = param[i].Substring(param[i].IndexOf("_")+1);
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
            Filters cmftrepcF = new Filters { filter_id = "cmftrepc", code = "cmftrepc", name = "Страна производитель" };
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

            return filters;
        }
    }
}
