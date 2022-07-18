using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YGK
{
   class Block : GridObject
   {
      [SerializeField] BlockEntity m_entity;
      [SerializeField] ColorLayer m_colorLayer;
      [SerializeField] int m_initialCustomHP;

      public BlockEntity Entity => m_entity;
      public ColorLayer ColorLayer => m_colorLayer;
      
      /// <summary>
      /// Initialize block
      /// </summary>
      public void SetEntity(BlockEntity src)
      {
         m_entity = src;
         SetHP(m_initialCustomHP == 0 ? src.HP : m_initialCustomHP);
         SetColorLayer(m_entity.ColorLayer);
      }

      public void SetColorLayer(ColorLayer layer)
      {
         if (layer == ColorLayer.None)
            return;

         GetComponentInChildren<SpriteRenderer>().color = ColorManager.Instance.GetColor(layer);
         m_colorLayer = layer;
      }

      /// <summary>
      /// Not set layer (layer is used for matching)
      /// </summary>
      /// <param name="name"></param>
      public void SetRenderColorByName(string name)
      {
         GetComponentInChildren<SpriteRenderer>().color = ColorManager.Instance.GetColorByString(name);
      }

      public bool CanMatchWith(Block other)
      {
         return 
            m_entity.CanMatch && other.m_entity.CanMatch &&
            ((m_colorLayer & other.m_colorLayer) > 0);
      }
   }
}