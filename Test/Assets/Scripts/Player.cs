using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// /aaaaaaaaa
    /// /aaaaaaaaa
    /// </summary>
    public class Player : Hand
    {
        public float xPos, yPos;
        private bool isHouse;
        public void Initialise(float x, float y, bool houseOrPlayer)
        {
            base.Initialise();
            xPos = x;
            yPos = y;
            isHouse = houseOrPlayer;
        }

        public override void Add(Card c)
        {
            base.Add(c);
            int size = cards.Count;
            for(int i = 0; i < size; i++)
            {
                if (i < size - 1)
                {
                    iTween.MoveTo(cards[i].gameObject, new Vector2(cards[i].transform.position.x - .2f, yPos), 1f);
                }
                else
                {
                    iTween.MoveTo(cards[i].gameObject, new Vector2(xPos + (i * (.2f)), yPos), 1f);
                }

                if (isHouse)
                {
                    cards[i].Hide();
                }
                else
                {
                    cards[i].Show();
                }
                //if(!cards[i].isFaceUp && (!isHouse || i != 1)) ;
            } 
        }

        public int GetValue()
        {
        //Sums card value
            int val = 0;
            bool hasAce = false;
            foreach(Card c in cards)
            {
                if (c.isFaceUp)
                {
                    if (c.Rank >= Card.Ranks.Ten) val += 10;
                    else val += (int)c.Rank + 1;

                    if (c.Rank == Card.Ranks.Ace) hasAce = true;
                } 
            }

            if (hasAce && val <= 11) val += 10;

            if (val >= 0 && val < 21) return val;
            else if (val > 21) return -1; //bust
            else return -2;
        }

        public bool HouseHitting()
        {
            //Dealer will take another <= 16 but wont at 17
            FlipFaceUp();
            int val = GetValue();
            if (val >= 0 && val <= 16) return true;

            return false;
        }
    }
}

