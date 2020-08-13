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
        public bool isHouse;
        public bool isTrash;
        public string name;

        public void Initialise(float x, float y, bool houseOrPlayer, bool isTrash, string name)
        {
            base.Initialise();
            xPos = x;
            yPos = y;
            isHouse = houseOrPlayer;
            this.isTrash = isTrash;
            this.name = name;
        }

        public Card PlayCard(Card c)
        {
            foreach(var tmp in this.cards)
            {
                if(c == null || tmp.Rank > c.Rank)
                {
                    return tmp;
                }
            }
            return null;
        }

        public void Add(Card c)
        {
            base.Add(c,this);
            if (!isTrash)
            {
                Sort();
            }
               
            int size = cards.Count;
            for (int i = 0; i < size; i++)
            {
                float cardDistance = 1f;
                if (isHouse && !isTrash)
                {
                    cardDistance = 0.3f;
                }


                cards[i].GetComponent<SpriteRenderer>().sortingOrder = i;

                if (!isTrash)
                {
                    if (size / 2 <= i - 1)
                    {
                        iTween.MoveTo(cards[i].gameObject, new Vector2(cardDistance * (i - (size / 2)) + xPos, yPos), 1f);
                    }
                    else
                    {
                        iTween.MoveTo(cards[i].gameObject, new Vector2(-1 * cardDistance * ((size / 2) - i) + xPos, yPos), 1f);
                    }
                }
                else
                {
                    iTween.MoveTo(cards[i].gameObject, new Vector2(0,0), 1f);
                }
                

                if (isHouse && !isTrash)
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

