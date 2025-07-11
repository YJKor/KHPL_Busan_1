using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NPC_Talk : XRBaseInteractable
{
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        var interactor = args.interactorObject as XRBaseInteractor;
        if (interactor != null)
        {
            Debug.Log("¿€µøµ ");
            Vector3 targetPosition = interactor.transform.position;
            Vector3 myPosition = transform.position;
            targetPosition.y = myPosition.y;
            transform.LookAt(targetPosition);
        }
    }
}