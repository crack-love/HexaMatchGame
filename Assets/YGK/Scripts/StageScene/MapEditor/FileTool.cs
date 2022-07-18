#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityCommon;
using System.Collections.Generic;

namespace YGK.MapEditor
{
   // todo : set stage file inspector
   class FileTool : ToolbarItem
   {
      const string Dir = "Assets/YGK/Entities/Stages";
      const string FileName = "/0.Asset";

      [SerializeField] StageEntity m_selectedStage;
      EditorTable<StageEntity> m_table;
      List<StageEntity> m_assets;
      List<StageEntity> m_destroyMe;

      public override string name => "File";

      public override void OnSelected()
      {
         if (m_assets == null) m_assets = new List<StageEntity>();
         if (m_destroyMe == null) m_destroyMe = new List<StageEntity>();

         if (m_table == null)
         {
            m_table = new EditorTable<StageEntity>();
            m_table.Add(new EditorTable<StageEntity>.ColumnLabel()
            {
               HeaderText = "Name",
               Getter = (x) => x.name,
            });
            m_table.Add(new EditorTable<StageEntity>.ColumnButton()
            {
               HeaderText = "Load",
               ButtonText = "Load",
               Width = 100,
               OnClicked = (x) => SetEntity(x),
            });
            m_table.Add(new EditorTable<StageEntity>.ColumnRemoveButton()
            {
               OnRemove = (x) =>
               {
                  m_destroyMe.Add(x);
               }
            });
         }

         RefreshTableList();
      }

      void RefreshTableList()
      {
         m_assets = AssetDatabaseEx.LoadAllAssetsAtDir(m_assets, Dir);
         m_table.Rows = m_assets;
      }

      public override void OnGUI()
      {
         m_table.OnGUI();

         if (GUILayout.Button("New Stage"))
         {
            // create
            StageEntity o = CreateInstance<StageEntity>();
            o.OnValidate();
            o.SetEnableAllSockets(true);

            // save
            string upath = AssetDatabase.GenerateUniqueAssetPath(Dir + FileName);
            AssetDatabase.CreateAsset(o, upath);

            // reload
            o = AssetDatabase.LoadAssetAtPath<StageEntity>(upath);
            SetEntity(o);

            m_assets.Add(o);
         }

         // object field
         EditorGUI.BeginChangeCheck();
         StageEntity obj = EditorGUILayout.ObjectField("Map Entity", m_selectedStage, typeof(StageEntity), false) as StageEntity;
         if (EditorGUI.EndChangeCheck() && obj != m_selectedStage)
         {
            // entity changed
            SetEntity(obj);
         }

         foreach (var e in m_destroyMe)
         {
            m_assets.Remove(e);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(e));
         }
         m_destroyMe.Clear();
      }

      void SetEntity(StageEntity e)
      {
         m_selectedStage = e;
         GlobalVariables.Instance.CurrentStage = e;

         // set scene
         var stage = Stage.Instance;
         stage.LoadStage(e);
      }
   }
}
#endif