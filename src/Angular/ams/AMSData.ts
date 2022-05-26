export class ReceivedAMS {
  public key: string|null=null;
  public value: AMSData|null=null;

}

export class AMSData {

  constructor(copyData: Partial<AMSData> | null = null) {
    if (copyData != null) {
      Object.keys(copyData).forEach((key) => {
        (this as any)[key] = (copyData as any)[key];
      });
    }
  }
  public version: string|null = null;
  public authors: string|null = null;
  public sourceCommit: string|null = null;
  public ciSourceControl: string|null = null;
  public assemblyName: string|null = null;
  public dateGenerated: string|null = null;
  public commitId: string|null = null;
  public repoUrl: string|null = null;

  public get TheDate(): Date|null{
    if(this.dateGenerated == null)
      return null;
    return new Date(this.dateGenerated);
  }
}
