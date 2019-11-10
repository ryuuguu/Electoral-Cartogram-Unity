using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CandidateRecord : MonoBehaviour {

        public CandidateResult candidateResult;

        public TMP_Text candidateName;
        public Image partyColor;
        public TMP_Text partyName;
        public TMP_Text percentVote;
    

        
        public void SetCandidateResult(CandidateResult cr) {
                candidateName.text = cr.surname + " , " + cr.givenName + " , " + cr.middleName;
                var pd = PartyController.GetPartyData(cr.partyId);
                partyColor.color = pd.color;
                partyName.text = LanguageController.ChooseName(pd.names);
                percentVote.text = cr.percentVotes.ToString();
        }
}
