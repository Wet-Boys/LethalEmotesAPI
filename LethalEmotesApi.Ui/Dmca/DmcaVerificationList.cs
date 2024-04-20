using System.Collections.Generic;
using System.Linq;
using LethalEmotesApi.Ui.Elements.Recycle;

namespace LethalEmotesApi.Ui.Dmca;

public class DmcaVerificationList : RecycleListView<DmcaVerificationListItem, EmoteDmcaVerificationStatusDb.VerificationStatus>
{
    public EmoteDmcaVerificationStatusDb? verificationStatusDb;
    
    protected override IList<EmoteDmcaVerificationStatusDb.VerificationStatus> ListData =>
        verificationStatusDb!.GetVerificationData(EmoteUiManager.EmoteDb.EmoteModGuids.ToArray());
    
    protected override void OnInstantiateListItem(DmcaVerificationListItem instance) { }
}