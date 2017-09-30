using System.Collections.Generic;
using System.Linq;
using HSYErpBase.EntityDefinition;
using HSYErpBase.NHibernate;
using NHibernate;

namespace HSYErpBase.Wcf
{
    /// <summary>
    /// 该类可以用作几乎所有类型的基类，只要简单的调用对应的方法就好了
    /// </summary>
    public class BusinessEntityApi
    {
        public static WcfServicePayload<List<TEntity>> CombinedQuery<TEntity, TPropertyMap>(ISession hSession,
            Dictionary<ErpPropertyInfo, string> criteria,bool containsEndDate=false) 
            where TEntity : BusinessEntity, new() where TPropertyMap:PropertyEntityMapBase,new()
      
        {
            string businessEntityName = typeof (TEntity).Name;

            var type = EntityStructureApi.GetStructureByTypeName(businessEntityName);
            if (type == null)
            {
                return new WcfServicePayload<List<TEntity>>(WcfError.UnknownError, "TypeError:No type " + businessEntityName);
            }

            //对每个condition都找出所有满足条件的id，然后做join
            //HQL不支持对没有标记one to many的表做join，因此目前只能先全部查询一遍，然后在代码里做join
            //TODO:这里是日后优化的目标
            List<int> intersectKeys = null;
            foreach (var conEntry in criteria)
            {
                var query =
                    hSession.CreateQuery("from " + type.PropertyMapType +
                                         " where value = ? and PropertyId =" +
                                         conEntry.Key.PropertyId+(containsEndDate? "":" and end_date is null"));
                query.SetString(0, PropertyHelper.ParseValue(conEntry.Key, conEntry.Value));

                List<int> keys = query.List<TPropertyMap>().Select(q => q.EntityId).ToList();
                intersectKeys = intersectKeys?.Intersect(keys).ToList() ?? keys;
                if (intersectKeys.Count == 0)
                {
                    break;
                }
            }

            if (intersectKeys != null && intersectKeys.Count != 0)
            {
                List<TEntity> entries =
                    intersectKeys.Select(key => hSession.Get<TEntity>(key)).ToList();
                return new WcfServicePayload<List<TEntity>>(entries);
            }
            else
            {
                return new WcfServicePayload<List<TEntity>>(new List<TEntity>());
            }

        }

        /// <summary>
        /// 该方法返回所有EndDate不是Null的BusinessEntity，并附带空的Properties
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="hSession"></param>
        /// <returns></returns>
        public static WcfServicePayload<List<TEntity>> ListActive<TEntity>(ISession hSession) where TEntity : BasicEntity, new()
        {
            string businessEntityName = typeof(TEntity).Name;

            var type = EntityStructureApi.GetStructureByTypeName(businessEntityName);
            if (type == null)
            {
                return new WcfServicePayload<List<TEntity>>(WcfError.UnknownError, "TypeError:No type " + businessEntityName);
            }

            var query =
                        hSession.CreateQuery("from "+businessEntityName+ " where end_date is null");
            var result = query.List<TEntity>().ToList();
            
            return new WcfServicePayload<List<TEntity>>(result);
            
        }


        /// <summary>
        /// 该方法返回所有的BusinessEntity，并附带空的Properties。即便其EndDate不是Null。
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="hSession"></param>
        /// <returns></returns>
        public static WcfServicePayload<List<TEntity>> ListAll<TEntity>(ISession hSession)
            where TEntity : BasicEntity, new()
        {
            string businessEntityName = typeof(TEntity).Name;

            var type = EntityStructureApi.GetStructureByTypeName(businessEntityName);
            if (type == null)
            {
                return new WcfServicePayload<List<TEntity>>(WcfError.UnknownError, "TypeError:No type " + businessEntityName);
            }

            var result = hSession.CreateCriteria<TEntity>().List<TEntity>().ToList();

            return new WcfServicePayload<List<TEntity>>(result);
        }

        /// <summary>
        /// 根据Id获得一个BusinessEntity的详细数据并且不指定TPropertyMap，这样在获取时不会尝试去XXX_MAP中查找，适用于不需要查找或没有PropertyMap的BusinessEntity。
        /// </summary>
        /// <typeparam name="TEntity">要获取内容的BusinessEntity的类型</typeparam>
        /// <param name="hSession">NHibernateSession</param>
        /// <param name="id">BusinessEntity的Id</param>
        /// <returns>获取的实体/错误信息</returns>
        public static WcfServicePayload<TEntity> GetDetail<TEntity>(ISession hSession, int id)
            where TEntity : BasicEntity, new()
        {
            string name = typeof(TEntity).Name;

            var type = EntityStructureApi.GetStructureByTypeName(name);
            if (type == null)
            {
                return new WcfServicePayload<TEntity>(WcfError.UnknownError, "TypeError:No type " + name);
            }

            var entityStub = hSession.Get<TEntity>(id);

            if (entityStub == null)
            {
                return new WcfServicePayload<TEntity>(WcfError.UnknownError, "No such id:" + id);

            }

            if (entityStub.EntityTypeId > 0 && entityStub.EntityTypeId != type.Id)
            {
                return new WcfServicePayload<TEntity>(WcfError.UnknownError, "Type not match:" + entityStub.EntityTypeId);
            }

            return new WcfServicePayload<TEntity>(entityStub);
        }

        public static WcfServicePayload<TEntity> GetDetail<TPropertyMap, TEntity>(ISession hSession, int id,
            List<ErpPropertyInfo> propertyList = null)
            where TEntity : BusinessEntity, new() where TPropertyMap : PropertyEntityMapBase, new()
        {
            string name = typeof (TEntity).Name;
            string propMapName = typeof (TPropertyMap).Name;

            var type = EntityStructureApi.GetStructureByTypeName(name);
            if (type == null)
            {
                return new WcfServicePayload<TEntity>(WcfError.UnknownError, "TypeError:No type " + name);
            }

            var entityStub = hSession.Get<TEntity>(id);

            if (entityStub == null)
            {
                return new WcfServicePayload<TEntity>(WcfError.UnknownError, "No such id:" + id);

            }

            if (entityStub.EntityTypeId > 0 && entityStub.EntityTypeId != type.Id)
            {
                return new WcfServicePayload<TEntity>(WcfError.UnknownError, "Type not match:" + entityStub.EntityTypeId);
            }

            var info = PropertyHelper.GetPropertyForEntity(hSession, type.Id);

            var propQuery =
                hSession.CreateQuery("from " + propMapName + " where end_date is null and EntityId = " + entityStub.Id);
            var values = propQuery.List();
            foreach (var value in values)
            {
                if (value is PropertyEntityMapBase)
                {
                    var mapBase = value as PropertyEntityMapBase;
                    var proInfo = info.FirstOrDefault(a => a.PropertyId == mapBase.PropertyId);
                    if (proInfo != null)
                    {
                        entityStub.Properties.AddValueByPropertyInfo(proInfo,
                            PropertyHelper.FormatValue(proInfo, mapBase.Value));
                    }
                }
            }


            return new WcfServicePayload<TEntity>(entityStub);
        }

        public static WcfServicePayload<TEntity> Upsert<TPropertyMap, TEntity>(ISession hSession, TEntity graph, List<ErpPropertyInfo> propertyList = null)
            where TEntity : BusinessEntity, new() where TPropertyMap : PropertyEntityMapBase, new()
        {
            if (graph == null)
            {
                return new WcfServicePayload<TEntity>(WcfError.UnknownError, "Null Graph");
            }
            if (graph.Id > 0)
            {
                var value = hSession.Get<TEntity>(graph.Id);
                if (value != null)
                {
                    hSession.Evict(value);
                    return Update<TPropertyMap, TEntity>(hSession, graph, propertyList);
                }
            }

            return Add<TPropertyMap, TEntity>(hSession, graph, propertyList);
        }

        public static WcfServicePayload<TEntity> Upsert<TEntity>(ISession hSession, TEntity graph)
            where TEntity : BasicEntity, new() 
        {
            if (graph == null)
            {
                return new WcfServicePayload<TEntity>(WcfError.UnknownError, "Null Graph");
            }
            if (graph.Id > 0)
            {
                var value = hSession.Get<TEntity>(graph.Id);
                if (value != null)
                {
                    hSession.Evict(value);
                    return Update<TEntity>(hSession, graph);
                }
            }

            return Add<TEntity>(hSession, graph);
        }

        public static WcfServicePayload<TEntity> Add<TPropertyMap, TEntity>(ISession hSession, TEntity graph, List<ErpPropertyInfo> propertyList = null)
             where TEntity : BusinessEntity, new() where TPropertyMap : PropertyEntityMapBase, new()
        {
            if (graph == null)
            {
                return new WcfServicePayload<TEntity>(WcfError.UnknownError, "Null Graph");
            }
            if (graph.Id > 0)
            {
               var value= hSession.Get<TEntity>(graph.Id);
                if (value != null)
                {
                    //Add error!
                    return new WcfServicePayload<TEntity>(WcfError.AddDuplicate, null);

                }
            }

            int key=(int)hSession.Save(graph);
            graph.Id = key;

            var properties = graph.Properties;
            if (properties != null && properties.Count > 0)
            {
                UpsertProperties<TEntity, TPropertyMap>(hSession, graph.Id, properties, propertyList);
            }

            hSession.Flush();
            return new WcfServicePayload<TEntity>(graph);
        }

        public static WcfServicePayload<TEntity> Add<TEntity>(ISession hSession, TEntity graph)
             where TEntity : BasicEntity, new() 
        {
            if (graph == null)
            {
                return new WcfServicePayload<TEntity>(WcfError.UnknownError, "Null Graph");
            }
            if (graph.Id > 0)
            {
                var value = hSession.Get<TEntity>(graph.Id);
                if (value != null)
                {
                    //Add error!
                    return new WcfServicePayload<TEntity>(WcfError.AddDuplicate, null);

                }
            }

            int key = (int)hSession.Save(graph);
            graph.Id = key;
            
            hSession.Flush();
            return new WcfServicePayload<TEntity>(graph);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPropertyMap"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="hSession"></param>
        /// <param name="graph"></param>
        /// <param name="propertyList"></param>
        /// <returns></returns>
        public static WcfServicePayload<TEntity> Update<TPropertyMap, TEntity>(ISession hSession, TEntity graph, List<ErpPropertyInfo> propertyList=null)
            where TEntity : BusinessEntity, new() where TPropertyMap : PropertyEntityMapBase, new()
        {
            string name = typeof(TEntity).Name;
            var type = EntityStructureApi.GetStructureByTypeName(name);
            if (type == null)
            {
                return new WcfServicePayload<TEntity>(WcfError.UnknownError, "TypeError:No type " + name);
            }

            try
            {
                hSession.Update(graph);
                hSession.Flush();
            }
            catch (StaleStateException)
            {
                //Update error!
                return new WcfServicePayload<TEntity>(WcfError.UpdateMiss, null);
            }

            var properties = graph.Properties;
            if (properties != null && properties.Count > 0)
            {
                UpsertProperties<TEntity,TPropertyMap>(hSession, graph.Id, properties, propertyList);
            }

            hSession.Flush();
            return new WcfServicePayload<TEntity>(graph);
        }

        public static WcfServicePayload<TEntity> Update<TEntity>(ISession hSession, TEntity graph, List<ErpPropertyInfo> propertyList = null)
            where TEntity : BasicEntity, new() 
        {
            string name = typeof(TEntity).Name;
            var type = EntityStructureApi.GetStructureByTypeName(name);
            if (type == null)
            {
                return new WcfServicePayload<TEntity>(WcfError.UnknownError, "TypeError:No type " + name);
            }

            try
            {
                hSession.Update(graph);
                hSession.Flush();
            }
            catch (StaleStateException)
            {
                //Update error!
                return new WcfServicePayload<TEntity>(WcfError.UpdateMiss, null);
            }
            
            hSession.Flush();
            return new WcfServicePayload<TEntity>(graph);
        }

        /// <summary>
        /// 该函数目前有严重bug，并未处理相同属性为多个的情况。
        /// </summary>
        /// <typeparam name="TPropertyMap"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="hSession"></param>
        /// <param name="entityId"></param>
        /// <param name="properties"></param>
        /// <param name="propertyList"></param>
        private static void UpsertProperties<TEntity, TPropertyMap>(ISession hSession, int entityId,
            List<BusinessEntityPropertyItem> properties, List<ErpPropertyInfo> propertyList)
            where TEntity : BusinessEntity, new() where TPropertyMap : PropertyEntityMapBase, new()
        {
            string propertyMapClassName = typeof (TPropertyMap).Name;

            Dictionary<int, List<BusinessEntityPropertyItem>> groupByResult =
                new Dictionary<int, List<BusinessEntityPropertyItem>>();

            List<ErpPropertyInfo> allPropertyCache = null;

            foreach (var item in properties)
            {
                if (item.PropertyInfo.PropertyId <= 0)
                {
                    if (allPropertyCache == null)
                    {
                        if (propertyList != null)
                        {
                            allPropertyCache = propertyList;
                        }
                        else
                        {
                            allPropertyCache = QueryProperty<TEntity>(hSession)?.Payload ?? new List<ErpPropertyInfo>();
                        }
                    }

                    foreach (var erpPropertyInfo in allPropertyCache)
                    {
                        if (erpPropertyInfo.Name ==
                            item.PropertyInfo.Name)
                        {
                            item.PropertyInfo.PropertyId = erpPropertyInfo.PropertyId;
                        }
                    }

                }

                if (!groupByResult.ContainsKey(item.PropertyInfo.PropertyId))
                {
                    groupByResult.Add(item.PropertyInfo.PropertyId, new List<BusinessEntityPropertyItem>());
                }

                groupByResult[item.PropertyInfo.PropertyId].Add(item);
            }

            foreach (var group in groupByResult)
            {
                if (group.Value.Count == 1)
                {
                    var propEntry = group.Value.First();

                    TPropertyMap map = new TPropertyMap();
                    map.EntityId = entityId;
                    map.PropertyId = propEntry.PropertyInfo.PropertyId;
                    map.Value = PropertyHelper.ParseValue(propEntry.PropertyInfo, propEntry.Value);

                    var query =
                        hSession.CreateQuery("update " + propertyMapClassName + " set Value= ? where PropertyId=" +
                                             propEntry.PropertyInfo.PropertyId + " and EntityId=" + entityId);
                    query.SetString(0, map.Value);
                    var x = query.ExecuteUpdate();
                    if (x == 0)
                    {
                        hSession.Save(map);
                    }
                }
                else if (group.Value.Count > 1)
                {
                    //TODO:不止一个的时候，要进行比对，数据库里不存在的要创建，给定列表里不存在的要Retire
                }
            }
        }

        public static WcfServicePayload Retire<TEntity>(ISession hSession, int? id)
            where TEntity : BasicEntity, new()
        {
            if (id == null)
            {
                return new WcfServicePayload(WcfError.DeleteMiss, "Retire Miss");
            }
            string entityName = typeof(TEntity).Name;
            try
            {
                var query = hSession.CreateQuery("update "+ entityName+" set end_date = sysdate where Id = " +
                                             id);
                int count=query.ExecuteUpdate();

                if (count == 0)
                {
                    return new WcfServicePayload(WcfError.DeleteMiss, "Retire Miss");
                }
                hSession.Flush();
                return new WcfServicePayload(WcfError.None, null);
            }
            catch (StaleStateException)
            {
                //Update error!
                return new WcfServicePayload(WcfError.DeleteMiss, null);
            }
        }

        public static WcfServicePayload Purge<TEntity>(ISession hSession, int? id)
            where TEntity : BasicEntity, new()
        {
            if (id == null)
            {
                return new WcfServicePayload(WcfError.DeleteMiss, "Retire Miss");
            }
            string entityName = typeof(TEntity).Name;
            try
            {
                var query = hSession.CreateQuery("delete "+ entityName+" where Id = " +
                                             id);
                int count = query.ExecuteUpdate();

                if (count == 0)
                {
                    return new WcfServicePayload(WcfError.DeleteMiss, "Retire Miss");
                }
                hSession.Flush();
                return new WcfServicePayload(WcfError.None, null);
            }
            catch (StaleStateException)
            {
                //Update error!
                return new WcfServicePayload(WcfError.DeleteMiss, null);
            }
        }

        public static WcfServicePayload<List<ErpPropertyInfo>> QueryProperty<T>(ISession hSession)
        {
            string name = typeof(T).Name;
            var type = EntityStructureApi.GetStructureByTypeName(name);
            if (type == null)
            {
                return new WcfServicePayload<List<ErpPropertyInfo>>(WcfError.UnknownError, "TypeError:No type " + name);
            }

            var info = PropertyHelper.GetPropertyForEntity(hSession, type.Id);

            return new WcfServicePayload<List<ErpPropertyInfo>>(info);
            
        }
        
        /// <summary>
        /// 该函数可以根据一个BusinessEntity，创建一个新的T类型并填入BusinessEntity的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityStub"></param>
        /// <returns></returns>
        private static T CreateFinalOutput<T>(BusinessEntity entityStub) where T : BusinessEntity, new()
        {
            T finalOutput = new T
            {
                Id = entityStub.Id,
                EntityTypeId = entityStub.EntityTypeId,
                BeginDate = entityStub.BeginDate,
                EndDate = entityStub.EndDate,
                Properties = entityStub.Properties

            };

            return finalOutput;
        }
    }
}